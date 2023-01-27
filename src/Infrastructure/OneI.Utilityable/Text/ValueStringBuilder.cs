namespace OneI.Text;

/// <summary>
/// source: <see href="https://github.com/dotnet/runtime/blob/6009a1064ccfc2bd9aeb96c9247b60cf6352198d/src/libraries/Common/src/System/Text/ValueStringBuilder.cs"/>
/// </summary>
public partial struct ValueStringBuilder
{
    private int _pos;
    private Memory<char> _chars;

    public ValueStringBuilder(int initialCapacity)
    {
        _chars = new char[initialCapacity];
        _pos = 0;
    }

    public int Length
    {
        get => _pos;
        set
        {
            Debug.Assert(value >= 0);
            Debug.Assert(value <= _chars.Length);
            _pos = value;
        }
    }

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

    public override string ToString()
    {
        var result = new string(_chars.ToArray());

        Dispose();

        return result;
    }

    /// <summary>Returns the underlying storage of the builder.</summary>
    public Span<char> RawChars => _chars.Span;

    #region Insert

    public void Insert(int index, char value, int count)
    {
        if(_pos > _chars.Length - count)
        {
            Grow(count);
        }

        var remaining = _pos - index;
        _chars[index..remaining].CopyTo(_chars[(index + count)..]);
        _chars.Span[index..count].Fill(value);
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

        _chars[index..remaining].CopyTo(_chars[(index + count)..]);

        s.CopyTo(_chars[index..].Span);

        _pos += count;
    }
    #endregion Insert End

    #region Append

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(char c)
    {
        var pos = _pos;
        if((uint)pos < (uint)_chars.Length)
        {
            _chars.Span[pos] = c;
            _pos = pos + 1;
        }
        else
        {
            GrowAndAppend(c);
        }
    }

    public void Append(char[] buffer, int index, int count)
    {
        Append(buffer[index..count]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(string? s)
    {
        if(s == null)
        {
            return;
        }

        var pos = _pos;
        if(s.Length == 1 && (uint)pos < (uint)_chars.Length) // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
        {
            _chars.Span[pos] = s[0];
            _pos = pos + 1;
        }
        else
        {
            AppendSlow(s);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(char c, int count)
    {
        Insert(_pos, c, count);
    }

    public void Append(scoped ReadOnlySpan<char> value)
    {
        var pos = _pos;

        if(pos > _chars.Length - value.Length)
        {
            Grow(value.Length);
        }

        value.CopyTo(_chars.Span[_pos..]);

        _pos += value.Length;
    }

    public void Append<T>(T value, string? format = null, IFormatProvider? provider = null)
        where T : ISpanFormattable
    {
        if(value.TryFormat(_chars.Span[_pos..], out var charsWritten, format, provider))
        {
            _pos += charsWritten;
        }
        else
        {
            Append(value.ToString(format, provider));
        }
    }

    public void Append(StringBuilder? value)
        => Append(value, 0, value?.Length ?? 0);

    public void Append(StringBuilder? value, int startIndex, int count)
    {
        if(value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var newLength = Length + count;

        if((uint)newLength > SharedConstants.ArrayMaxLength)
        {
            throw new ArgumentOutOfRangeException(nameof(Capacity), "Capacity exceeds maximum capacity.");
        }

        value.CopyTo(startIndex, _chars.Span, count);
    }

    public void AppendLine() => Append(Environment.NewLine);

    public void AppendLine(string? value)
    {
        Append(value);

        Append(Environment.NewLine);
    }

    private void AppendSlow(string s)
    {
        var pos = _pos;
        if(pos > _chars.Length - s.Length)
        {
            Grow(s.Length);
        }

        s.CopyTo(_chars.Span[pos..]);

        _pos += s.Length;
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
        Debug.Assert(_pos > _chars.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

        // Increase to at least the required size (_pos + additionalCapacityBeyondPos), but try
        // to double the size if possible, bounding the doubling to not go beyond the max array length.
        var newCapacity = (int)Math.Max(
            (uint)(_pos + additionalCapacityBeyondPos),
            Math.Min((uint)_chars.Length * 2, SharedConstants.ArrayMaxLength));

        var poolArray = new char[newCapacity];

        _chars.Span[.._pos].CopyTo(poolArray);

        _chars = poolArray;
    }

    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        this = default; // for safety, to avoid using pooled array if this instance is erroneously appended to again
    }
}
