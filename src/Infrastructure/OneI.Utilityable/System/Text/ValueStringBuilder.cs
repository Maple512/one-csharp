namespace System.Text;

using System;
using System.Buffers;
using System.Runtime.InteropServices;
using DotNext;
using OneI;

// https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/System/Text/ValueStringBuilder.cs
public struct ValueStringBuilder
{
    private char[]? _arrayToReturnToPool;
    private Memory<char> _chars;
    private int _pos;

    public ValueStringBuilder(scoped Span<char> buffer)
    {
        _arrayToReturnToPool = null;
        var span = AppendSpan(buffer.Length);
        buffer.TryCopyTo(span);
        _pos = 0;
    }

    public ValueStringBuilder(Memory<char> initialBuffer)
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

    public readonly Span<char> GetSpan() => _chars.Span;

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

    public ref char GetPinnableReference()
    {
        return ref MemoryMarshal.GetReference(GetSpan());
    }

    public ref char GetPinnableReference(bool terminate)
    {
        if(terminate)
        {
            EnsureCapacity(Length + 1);

            GetSpan()[Length] = '\0';
        }

        return ref MemoryMarshal.GetReference(GetSpan());
    }

    public ref char this[int index]
    {
        get
        {
            Debug.Assert(index < _pos);

            return ref GetSpan()[index];
        }
    }

    public override string ToString()
    {
        var s = GetSpan()[.._pos].ToString();

        Dispose();

        return s;
    }

    public readonly Span<char> RawChars => GetSpan();

    public ReadOnlySpan<char> AsSpan(bool terminate)
    {
        if(terminate)
        {
            EnsureCapacity(Length + 1);

            GetSpan()[Length] = '\0';
        }

        return GetSpan()[.._pos];
    }

    public ReadOnlySpan<char> AsSpan() => GetSpan()[.._pos];
    public ReadOnlySpan<char> AsSpan(int start) => GetSpan()[start.._pos];
    public ReadOnlySpan<char> AsSpan(int start, int length) => GetSpan().Slice(start, length);

    public bool TryCopyTo(Memory<char> destination, out int charsWritten)
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

        GetSpan().Slice(index, remaining).CopyTo(GetSpan()[(index + count)..]);

        GetSpan().Slice(index, count).Fill(value);

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

        GetSpan().Slice(index, remaining).CopyTo(GetSpan()[(index + count)..]);

        s.CopyTo(GetSpan()[index..]);

        _pos += count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(char c)
    {
        var pos = _pos;
        if((uint)pos < (uint)_chars.Length)
        {
            GetSpan()[pos] = c;

            _pos = pos + 1;
        }
        else
        {
            GrowAndAppend(c);
        }
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
            GetSpan()[pos] = s[0];

            _pos = pos + 1;
        }
        else
        {
            AppendSlow(s);
        }
    }

    private void AppendSlow(string s)
    {
        var pos = _pos;
        if(pos > _chars.Length - s.Length)
        {
            Grow(s.Length);
        }

        s.CopyTo(GetSpan()[pos..]);
        _pos += s.Length;
    }

    public void Append(char c, int count)
    {
        if(_pos > _chars.Length - count)
        {
            Grow(count);
        }

        scoped Span<char> dst = GetSpan().Slice(_pos, count);

        for(var i = 0; i < dst.Length; i++)
        {
            dst[i] = c;
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

        var dst = GetSpan().Slice(_pos, length);
        for(var i = 0; i < dst.Length; i++)
        {
            dst[i] = *value++;
        }

        _pos += length;
    }

    public void Append(scoped in ReadOnlySpan<char> value)
    {
        var pos = _pos;
        if(pos > _chars.Length - value.Length)
        {
            Grow(value.Length);
        }

        value.CopyTo(GetSpan()[_pos..]);

        _pos += value.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<char> AppendSpan(int length)
    {
        var origPos = _pos;
        if(origPos > _chars.Length - length)
        {
            Grow(length);
        }

        _pos = origPos + length;

        return GetSpan().Slice(origPos, length);
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

        GetSpan()[.._pos].CopyTo(poolArray);

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

    public string AppendFormat([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] args)
    {
        Check.NotNullOrEmpty(format);
        Check.NotNull(args);

        return FormatHelper(null, format, args);
    }

    public string AppendFormat(IFormatProvider? provider, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] args)
    {
        Check.NotNullOrEmpty(format);
        Check.NotNull(args);

        return FormatHelper(provider, format, args);
    }

    private string FormatHelper(IFormatProvider? provider, string format, ReadOnlySpan<object?> args)
    {
        ArgumentNullException.ThrowIfNull(format);

        var sb = new ValueStringBuilder(stackalloc char[256]);
        sb.EnsureCapacity(format.Length + args.Length * 8);
        sb.AppendFormatHelper(provider, format, args);
        return sb.ToString();
    }

    internal void AppendFormatHelper(IFormatProvider? provider, string format, ReadOnlySpan<object?> args)
    {
        ArgumentNullException.ThrowIfNull(format);

        // Undocumented exclusive limits on the range for Argument Hole Index and Argument Hole Alignment.
        const int IndexLimit = 1_000_000; // Note:            0 <= ArgIndex < IndexLimit
        const int WidthLimit = 1_000_000; // Note:  -WidthLimit <  ArgAlign < WidthLimit

        // Query the provider (if one was supplied) for an ICustomFormatter.  If there is one,
        // it needs to be used to transform all arguments.
        var cf = (ICustomFormatter?)provider?.GetFormat(typeof(ICustomFormatter));

        // Repeatedly find the next hole and process it.
        int pos = 0;
        char ch;
        while(true)
        {
            // Skip until either the end of the input or the first unescaped opening brace, whichever comes first.
            // Along the way we need to also unescape escaped closing braces.
            while(true)
            {
                // Find the next brace.  If there isn't one, the remainder of the input is text to be appended, and we're done.
                if((uint)pos >= (uint)format.Length)
                {
                    return;
                }

                ReadOnlySpan<char> remainder = format.AsSpan(pos);
                int countUntilNextBrace = remainder.IndexOfAny('{', '}');
                if(countUntilNextBrace < 0)
                {
                    Append(remainder);
                    return;
                }

                // Append the text until the brace.
                Append(remainder[..countUntilNextBrace]);
                pos += countUntilNextBrace;

                // Get the brace.  It must be followed by another character, either a copy of itself in the case of being
                // escaped, or an arbitrary character that's part of the hole in the case of an opening brace.
                char brace = format[pos];
                ch = MoveNext(format, ref pos);
                if(brace == ch)
                {
                    Append(ch);
                    pos++;
                    continue;
                }

                // This wasn't an escape, so it must be an opening brace.
                if(brace != '{')
                {
                    ThrowFormatInvalidString();
                }

                // Proceed to parse the hole.
                break;
            }

            // We're now positioned just after the opening brace of an argument hole, which consists of
            // an opening brace, an index, an optional width preceded by a comma, and an optional format
            // preceded by a colon, with arbitrary amounts of spaces throughout.
            int width = 0;
            bool leftJustify = false;
            scoped ReadOnlySpan<char> itemFormatSpan = default; // used if itemFormat is null

            // First up is the index parameter, which is of the form:
            //     at least on digit
            //     optional any number of spaces
            // We've already read the first digit into ch.
            Debug.Assert(format[pos - 1] == '{');
            Debug.Assert(ch != '{');
            int index = ch - '0';
            if((uint)index >= 10u)
            {
                ThrowFormatInvalidString();
            }

            // Common case is a single digit index followed by a closing brace.  If it's not a closing brace,
            // proceed to finish parsing the full hole format.
            ch = MoveNext(format, ref pos);
            if(ch != '}')
            {
                // Continue consuming optional additional digits.
                while(char.IsAsciiDigit(ch) && index < IndexLimit)
                {
                    index = index * 10 + ch - '0';
                    ch = MoveNext(format, ref pos);
                }

                // Consume optional whitespace.
                while(ch == ' ')
                {
                    ch = MoveNext(format, ref pos);
                }

                // Parse the optional alignment, which is of the form:
                //     comma
                //     optional any number of spaces
                //     optional -
                //     at least one digit
                //     optional any number of spaces
                if(ch == ',')
                {
                    // Consume optional whitespace.
                    do
                    {
                        ch = MoveNext(format, ref pos);
                    }
                    while(ch == ' ');

                    // Consume an optional minus sign indicating left alignment.
                    if(ch == '-')
                    {
                        leftJustify = true;
                        ch = MoveNext(format, ref pos);
                    }

                    // Parse alignment digits. The read character must be a digit.
                    width = ch - '0';
                    if((uint)width >= 10u)
                    {
                        ThrowFormatInvalidString();
                    }
                    ch = MoveNext(format, ref pos);
                    while(char.IsAsciiDigit(ch) && width < WidthLimit)
                    {
                        width = width * 10 + ch - '0';
                        ch = MoveNext(format, ref pos);
                    }

                    // Consume optional whitespace
                    while(ch == ' ')
                    {
                        ch = MoveNext(format, ref pos);
                    }
                }

                // The next character needs to either be a closing brace for the end of the hole,
                // or a colon indicating the start of the format.
                if(ch != '}')
                {
                    if(ch != ':')
                    {
                        // Unexpected character
                        ThrowFormatInvalidString();
                    }

                    // Search for the closing brace; everything in between is the format,
                    // but opening braces aren't allowed.
                    int startingPos = pos;
                    while(true)
                    {
                        ch = MoveNext(format, ref pos);

                        if(ch == '}')
                        {
                            // Argument hole closed
                            break;
                        }

                        if(ch == '{')
                        {
                            // Braces inside the argument hole are not supported
                            ThrowFormatInvalidString();
                        }
                    }

                    startingPos++;
                    itemFormatSpan = format.AsSpan(startingPos, pos - startingPos);
                }
            }

            // Construct the output for this arg hole.
            Debug.Assert(format[pos] == '}');
            pos++;
            string? s = null;
            string? itemFormat = null;

            if((uint)index >= (uint)args.Length)
            {
                throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");
            }
            object? arg = args[index];

            if(cf != null)
            {
                if(!itemFormatSpan.IsEmpty)
                {
                    itemFormat = new string(itemFormatSpan);
                }

                s = cf.Format(itemFormat, arg, provider);
            }

            if(s == null)
            {
                // If arg is ISpanFormattable and the beginning doesn't need padding,
                // try formatting it into the remaining current chunk.
                if((leftJustify || width == 0) &&
                    arg is ISpanFormattable spanFormattableArg &&
                    spanFormattableArg.TryFormat(GetSpan()[_pos..], out int charsWritten, itemFormatSpan, provider))
                {
                    _pos += charsWritten;

                    // Pad the end, if needed.
                    if(leftJustify && width > charsWritten)
                    {
                        Append(' ', width - charsWritten);
                    }

                    // Continue to parse other characters.
                    continue;
                }

                // Otherwise, fallback to trying IFormattable or calling ToString.
                if(arg is IFormattable formattableArg)
                {
                    if(itemFormatSpan.Length != 0)
                    {
                        itemFormat ??= new string(itemFormatSpan);
                    }
                    s = formattableArg.ToString(itemFormat, provider);
                }
                else
                {
                    s = arg?.ToString();
                }

                s ??= string.Empty;
            }

            // Append it to the final output of the Format String.
            if(width <= s.Length)
            {
                Append(s);
            }
            else if(leftJustify)
            {
                Append(s);
                Append(' ', width - s.Length);
            }
            else
            {
                Append(' ', width - s.Length);
                Append(s);
            }

            // Continue parsing the rest of the format string.
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static char MoveNext(string format, ref int pos)
        {
            pos++;
            if((uint)pos >= (uint)format.Length)
            {
                ThrowFormatInvalidString();
            }

            return format[pos];
        }
    }

    private static void ThrowFormatInvalidString()
        => throw new FormatException("Input string was not in a correct format.");
}
