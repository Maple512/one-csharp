namespace OneI.Text;

using System;
using System.Buffers;
using System.ComponentModel;

/// <summary>
/// <see href="https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/System/Text/ValueStringBuilder.cs"/>
/// </summary>
public ref struct RefValueStringBuilder
{
    /// <summary>缓冲池最小长度</summary>
    private const int ArrayPoolMinimumLength = 256;

    private int _pos;
    private char[]? _arrayToReturnToPool;
    private Span<char> _buffer;

    public RefValueStringBuilder()
        : this(ArrayPoolMinimumLength)
    {
    }

    public RefValueStringBuilder(Span<char> initialBuffer)
    {
        _arrayToReturnToPool = null;
        _buffer = initialBuffer;
        _pos = 0;
    }

    public RefValueStringBuilder(int initialCapacity)
    {
        _arrayToReturnToPool = ArrayPool<char>.Shared.Rent(initialCapacity);
        _buffer = _arrayToReturnToPool;
        _pos = 0;
    }

    public int Length
    {
        get
        {
            return _pos;
        }

        set
        {
            Debug.Assert(value >= 0);
            Debug.Assert(value <= _buffer.Length);

            _pos = value;
        }
    }

    public int Capacity => _buffer.Length;

    public void EnsureCapacity(int capacity)
    {
        // This is not expected to be called this with negative capacity
        Debug.Assert(capacity >= 0);

        // If the caller has a bug and calls this with negative capacity, make sure to call Grow to throw an exception.
        if((uint)capacity > (uint)_buffer.Length)
        {
            Grow(capacity - _pos);
        }
    }

    /// <summary>
    /// Get a pinnable reference to the builder.
    /// Does not ensure there is a null char after <see cref="Length"/>
    /// This overload is pattern matched in the C# 7.3+ compiler so you can omit
    /// the explicit method call, and write eg "fixed (char* c = builder)"
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ref char GetPinnableReference()
    {
        return ref MemoryMarshal.GetReference(_buffer);
    }

    /// <summary>
    /// Get a pinnable reference to the builder.
    /// </summary>
    /// <param name="terminate">Ensures that the builder has a null char after <see cref="Length"/></param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ref char GetPinnableReference(bool terminate)
    {
        if(terminate)
        {
            EnsureCapacity(Length + 1);
            _buffer[Length] = '\0';
        }

        return ref MemoryMarshal.GetReference(_buffer);
    }

    public ref char this[int index]
    {
        get
        {
            Debug.Assert(index < _pos);
            return ref _buffer[index];
        }
    }

    public override string ToString()
    {
        var s = AsSpan().ToString();

        Dispose();

        return s;
    }

    /// <summary>Returns the underlying storage of the builder.</summary>
    public Span<char> RawChars => _buffer;

    #region AsSpan

    /// <summary>
    /// Returns a span around the contents of the builder.
    /// </summary>
    /// <param name="terminate">Ensures that the builder has a null char after <see cref="Length"/></param>
    public ReadOnlySpan<char> AsSpan(bool terminate)
    {
        if(terminate)
        {
            EnsureCapacity(Length + 1);
            _buffer[Length] = '\0';
        }

        return _buffer[.._pos];
    }

    public ReadOnlySpan<char> AsSpan() => _buffer[.._pos];
    public ReadOnlySpan<char> AsSpan(int start) => _buffer[start.._pos];
    public ReadOnlySpan<char> AsSpan(int start, int length) => _buffer.Slice(start, length);

    #endregion

    public bool TryCopyTo(Span<char> destination, out int charsWritten)
    {
        if(_buffer[.._pos].TryCopyTo(destination))
        {
            charsWritten = _pos;

            Dispose();

            return true;
        }

        charsWritten = 0;

        Dispose();

        return false;
    }

    #region Insert

    public void Insert(int index, char value, int count)
    {
        if(_pos > _buffer.Length - count)
        {
            Grow(count);
        }

        var remaining = _pos - index;
        _buffer.Slice(index, remaining).CopyTo(_buffer[(index + count)..]);
        _buffer.Slice(index, count).Fill(value);
        _pos += count;
    }

    public void Insert(int index, string? s)
    {
        if(s == null)
        {
            return;
        }

        var count = s.Length;

        if(_pos > _buffer.Length - count)
        {
            Grow(count);
        }

        var remaining = _pos - index;

        _buffer.Slice(index, remaining).CopyTo(_buffer[(index + count)..]);

        s.CopyTo(_buffer[index..]);

        _pos += count;
    }

    #endregion

    #region Replace

    public void Replace(char oldChar, char newChar) => Replace(oldChar, newChar, 0, Length);

    public void Replace(char oldChar, char newChar, int start, int count)
    {
        var currentLength = Length;
        if((uint)start > (uint)currentLength)
        {
            throw new ArgumentOutOfRangeException(nameof(start));
        }

        if(count < 0 || start > currentLength - count)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        var s = start;
        var c = count;
        while(true)
        {
            var index = _buffer.Slice(s, c).IndexOf(oldChar);

            if(index < 0)
            {
                break;
            }

            _buffer[index + s] = newChar;
            var len = index + 1;
            s += len;
            c -= len;
        }
    }

    #endregion

    #region Append

    public void Append(char c)
    {
        var pos = _pos;

        if((uint)pos < (uint)_buffer.Length)
        {
            _buffer[pos] = c;
            _pos = pos + 1;
        }
        else
        {
            GrowAndAppend(c);
        }
    }

    public void Append(string? s)
    {
        if(s == null)
        {
            return;
        }

        var pos = _pos;
        if(s.Length == 1 && (uint)pos < (uint)_buffer.Length) // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
        {
            _buffer[pos] = s[0];
            _pos = pos + 1;
        }
        else
        {
            Append(s.AsSpan());
        }
    }

    public void Append(string? value, int start, int length)
    {
        if(string.IsNullOrEmpty(value))
        {
            if(start == 0 && length == 0)
            {
                return;
            }
            else
            {
                throw new ArgumentNullException(nameof(value));
            }
        }

        Append(value.AsSpan(start, length));
    }

    public void Append(char c, int count)
    {
        if(_pos > _buffer.Length - count)
        {
            Grow(count);
        }

        scoped Span<char> span = stackalloc char[count];
        span.Fill(c);

        span.CopyTo(_buffer[_pos..]);

        _pos += count;
    }

    public void Append(ReadOnlySpan<char> value)
    {
        var pos = _pos;
        if(pos > _buffer.Length - value.Length)
        {
            Grow(value.Length);
        }

        value.CopyTo(_buffer[_pos..]);

        _pos += value.Length;
    }

    public void Append(Rune rune)
    {
        var pos = _pos;
        var chars = _buffer;
        if((uint)(pos + 1) < (uint)chars.Length && (uint)pos < (uint)chars.Length)
        {
            if(rune.Value <= 0xFFFF)
            {
                chars[pos] = (char)rune.Value;
                _pos = pos + 1;
            }
            else
            {
                chars[pos] = (char)(rune.Value + (0xD800u - 0x40u << 10) >> 10);
                chars[pos + 1] = (char)((rune.Value & 0x3FFu) + 0xDC00u);
                _pos = pos + 2;
            }
        }
        else
        {
            GrowAndAppend(rune);
        }
    }

    #endregion

    #region Append Line

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine() => Append(Environment.NewLine.AsSpan());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine(string? value)
    {
        Append(value);

        AppendLine();
    }

    #endregion

    #region Append Span Format

    public void AppendSpanFormattable<T>(T value, string? format = null, IFormatProvider? provider = null)
        where T : ISpanFormattable
    {
        if(value.TryFormat(_buffer[_pos..], out var charsWritten, format, provider))
        {
            _pos += charsWritten;
        }
        else
        {
            Append(value.ToString(format, provider));
        }
    }

    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        var toReturn = _arrayToReturnToPool;
        this = default; // for safety, to avoid using pooled array if this instance is erroneously appended to again
        if(toReturn != null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    #region Private

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(Rune rune)
    {
        Grow(2);

        Append(rune);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(char c)
    {
        Grow(1);
        Append(c);
    }

    /// <summary>
    /// Resize the internal buffer either by doubling current buffer size or
    /// by adding <paramref name="additionalCapacityBeyondPos"/> to
    /// <see cref="_pos"/> whichever is greater.
    /// </summary>
    /// <param name="additionalCapacityBeyondPos">
    /// Number of chars requested beyond current position.
    /// </param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int additionalCapacityBeyondPos)
    {
        Debug.Assert(additionalCapacityBeyondPos > 0);
        Debug.Assert(_pos > _buffer.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

        const uint ArrayMaxLength = 0x7FFFFFC7; // same as Array.MaxLength

        // Increase to at least the required size (_pos + additionalCapacityBeyondPos), but try
        // to double the size if possible, bounding the doubling to not go beyond the max array length.
        var newCapacity = (int)Math.Max(
            (uint)(_pos + additionalCapacityBeyondPos),
            Math.Min((uint)_buffer.Length * 2, ArrayMaxLength));

        // Make sure to let Rent throw an exception if the caller has a bug and the desired capacity is negative.
        // This could also go negative if the actual required length wraps around.
        var poolArray = ArrayPool<char>.Shared.Rent(newCapacity);

        _buffer[.._pos].CopyTo(poolArray);

        var toReturn = _arrayToReturnToPool;
        _buffer = _arrayToReturnToPool = poolArray;
        if(toReturn != null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

    #endregion
}
