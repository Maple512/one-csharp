namespace OneI.Text;

using System;
using System.Buffers;
using DotNext;
using OneI.Buffers;

// https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/System/Text/ValueStringBuilder.cs
public partial struct ValueStringBuilder
{
    private ValueBuffer<char> _chars;
    private char[]? _arrayToReturnToPool;
    private int _pos;

    public ValueStringBuilder(scoped Span<char> buffer)
    {
        _arrayToReturnToPool = null;
        _chars = buffer;
        _pos = 0;
    }

    public ValueStringBuilder(ValueBuffer<char> initialBuffer)
    {
        _arrayToReturnToPool = null;
        _chars = initialBuffer;
        _pos = 0;
    }

    public ValueStringBuilder(int initialCapacity)
    {
        _chars = _arrayToReturnToPool = ArrayPool<char>.Shared.Rent(initialCapacity);
        _pos = 0;
    }

    public int Length => _pos;

    public int Capacity => _chars.Length;

    public void EnsureCapacity(int capacity)
    {
        // This is not expected to be called this with negative capacity
        Debug.Assert(capacity >= 0);

        // If the caller has a bug and calls this with negative capacity, make sure to call Grow to throw an exception.
        if((uint)capacity > (uint)_chars.Length)
        {
            Grow(capacity - _pos);
        }
    }

    public ref readonly char this[int index]
    {
        get
        {
            Debug.Assert(index < _pos);

            return ref _chars[index];
        }
    }

    public override string ToString()
    {
        var s = _chars[.._pos].ToString();

        Dispose();

        return s;
    }

    public Span<char> AsSpan() => _chars[.._pos].AsSpan();

    public bool TryCopyTo(ValueBuffer<char> destination, out int charsWritten)
    {
        if(_chars[.._pos].TryCopyTo(destination))
        {
            charsWritten = _pos;
            Dispose();
            return true;
        }
        else
        {
            charsWritten = 0;
            Dispose();
            return false;
        }
    }

    public void Insert(int index, char value, int count)
    {
        if(_pos > _chars.Length - count)
        {
            Grow(count);
        }

        var remaining = _pos - index;

        _chars.Slice(index, remaining).CopyTo(_chars[(index + count)..]);

        _chars.Slice(index, count).Fill(value);

        _pos += count;
    }

    public void Insert(int index, string? s)
    {
        if(s == null)
        {
            return;
        }

        var count = s.Length;

        if(_pos > _chars.Length - count)
        {
            Grow(count);
        }

        var remaining = _pos - index;

        _chars.Slice(index, remaining).CopyTo(_chars[(index + count)..]);

        s.CopyTo(_chars[index..]);

        _pos += count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(char c)
    {
        var pos = _pos;
        if((uint)pos < (uint)_chars.Length)
        {
            _chars[pos] = c;

            _pos = pos + 1;
        }
        else
        {
            GrowAndAppend(c);
        }
    }

    private void AppendSlow(string s)
    {
        var pos = _pos;
        if(pos > _chars.Length - s.Length)
        {
            Grow(s.Length);
        }

        s.CopyTo(_chars[pos..]);
        _pos += s.Length;
    }

    public void Append(char c, int count)
    {
        var length = _chars.Length;
        if(_pos > _chars.Length - count)
        {
            Grow(count);
        }

        for(var i = 0; i < count; i++)
        {
            _chars[i + length] = c;
        }

        _pos += count;
    }

    public unsafe void Append(char* value, int length)
    {
        var pos = _pos;
        if(pos > _chars.Length - length)
        {
            Grow(length);
        }

        var dst = _chars.Slice(_pos, length);
        for(var i = 0; i < dst.Length; i++)
        {
            dst[i] = *value++;
        }

        _pos += length;
    }

    public void Append(scoped in ReadOnlySpan<char> value)
    {
        if(value.IsEmpty)
        {
            return;
        }

        var pos = _pos;
        if(pos > _chars.Length - value.Length)
        {
            Grow(value.Length);
        }

        value.CopyTo(_chars[_pos..]);

        _pos += value.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine() => Append(Environment.NewLine);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLine(string? value)
    {
        Append(value);

        Append(Environment.NewLine);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(char c)
    {
        Grow(1);

        Append(c);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int additionalCapacityBeyondPos)
    {
        Debug.Assert(additionalCapacityBeyondPos > 0);
        Debug.Assert(_pos > _chars.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

        const uint ArrayMaxLength = 0x7FFFFFC7; // same as Array.MaxLength

        // Increase to at least the required size (_pos + additionalCapacityBeyondPos), but try
        // to double the size if possible, bounding the doubling to not go beyond the max array length.
        var newCapacity = (int)Math.Max(
            (uint)(_pos + additionalCapacityBeyondPos),
            Math.Min((uint)_chars.Length * 2, ArrayMaxLength));

        // Make sure to let Rent throw an exception if the caller has a bug and the desired capacity is negative.
        // This could also go negative if the actual required length wraps around.
        var poolArray = ArrayPool<char>.Shared.Rent(newCapacity);

        _chars[.._pos].CopyTo(poolArray);

        var toReturn = _arrayToReturnToPool;
        _chars = _arrayToReturnToPool = poolArray;
        if(toReturn != null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }

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

    private static void ThrowFormatInvalidString()
        => throw new FormatException("Input string was not in a correct format.");
}
