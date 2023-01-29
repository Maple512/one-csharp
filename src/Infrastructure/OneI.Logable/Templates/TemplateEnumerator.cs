namespace OneI.Logable.Templates;

using System.Globalization;
using static TemplateConstants.Property;

public struct TemplateEnumerator : IEquatable<TemplateEnumerator>
{
    private static readonly char[] HoleDelimiters = { _close, _format, _indent, _align };
    private static readonly char[] TextDelimiters = { '{', '}' };
    private const char _open = '{';
    private const char _close = '}';
    private const char _format = ':';
    private const char _indent = '\'';
    private const char _align = ',';

    public readonly string Template;
    private TemplateHolder _current;
    private TemplateHolder _next;
    private readonly ushort _length;
    private ushort _position;

    public TemplateEnumerator(string template)
    {
        if(template is null or { Length: > ushort.MaxValue })
        {
            throw new ArgumentOutOfRangeException(nameof(template));
        }

        Template = template;
        _current = default;
        _length = (ushort)template.Length;
        _position = 0;
    }

    public bool MoveNext()
    {
        if(_position >= _length)
        {
            return false;
        }

        if(_next.Equals(TemplateHolder.Default) == false)
        {
            _current = _next;
            _next = TemplateHolder.Default;
            return true;
        }

        // 剩余
        var position = _position;

        while(true)
        {
            if(position >= _length)
            {
                break;
            }

            var length = (ushort)(_length - position);

            scoped var remainder = Template.AsSpan().Slice(position, length);

            // '}'
            var closeIndex = remainder.IndexOf(_close);
            if(closeIndex == -1)
            {
                break;
            }

            if(closeIndex < 2)
            {
                position += (ushort)(closeIndex + 1);
                continue;
            }

            // '{'
            var openIndex = remainder[..closeIndex].LastIndexOf(_open);

            // '****}'
            if(openIndex == -1)
            {
                position += (ushort)(closeIndex + 1);
                continue;
            }

            // !'{x}'
            if(openIndex > closeIndex
                || closeIndex == openIndex + 1)
            {
                position += (ushort)(Math.Max(openIndex, closeIndex) + 1);
                continue;
            }

            position += (ushort)openIndex;
            var property = remainder[(openIndex + 1)..closeIndex];

            if(TryParseProperty(property, position, out _next))
            {
                if(_position != position)
                {
                    length = (ushort)(position - _position);
                    _current = TemplateHolder.CreateText(_position, length);
                    _position = (ushort)(position + property.Length + 2);
                    return true;
                }

                _current = _next;
                _next = default;
                _position += (ushort)(closeIndex + 1);
                return true;
            }

            position += (ushort)(closeIndex + 1);
        }

        var len = _length - _position;
        _current = TemplateHolder.CreateText(_position, (ushort)len);
        _position = _length;

        return true;
    }

    public TemplateHolder Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _current;
    }

    public override string ToString()
    {
        return Template.ToString();
    }

    private static bool TryParseProperty(in ReadOnlySpan<char> text, ushort position, [NotNullWhen(true)] out TemplateHolder token)
    {
        token = default;

        if(text.IsEmpty)
        {
            return false;
        }

        var fi = text.IndexOf(_format);
        var ai = text.IndexOf(_align);
        var ii = text.IndexOf(_indent);

        if(fi == 0 || ai == 0 || ii == 0)
        {
            return false;
        }

        var end = text.Length;

        if(fi == -1)
        {
            fi = end;
        }

        if(ai == -1)
        {
            ai = end;
        }

        if(ii == -1)
        {
            ii = end;
        }

        ref readonly var typeChar = ref text[0];
        var type = typeChar switch
        {
            '@' => PropertyType.Serialize,
            '$' => PropertyType.Stringify,
            _ => PropertyType.None,
        };

        var start = type > PropertyType.None ? 1 : 0;

        var nameRange = default(Range);
        var formatRange = default(Range);
        var alignRange = default(Range);
        var indentRange = default(Range);

        if(ai > fi && ii > fi)
        {
            nameRange = new Range(start, fi);
            if(ai > ii)
            {
                formatRange = new Range(fi + 1, ii);
                alignRange = new Range(ai + 1, end);
                indentRange = new Range(ii + 1, ai);
            }
            else
            {
                formatRange = new Range(fi + 1, ai);
                alignRange = new Range(ai + 1, ii);
                indentRange = new Range(ii + 1, end);
            }
        }
        else if(fi > ai && fi > ii)
        {
            formatRange = new Range(fi + 1, end);
            if(ai > ii)
            {
                nameRange = new Range(start, ii);
                alignRange = new Range(ai + 1, fi);
                indentRange = new Range(ii + 1, ai);
            }
            else
            {
                nameRange = new Range(start, ai);
                alignRange = new Range(ai + 1, ii);
                indentRange = new Range(ii + 1, end);
            }
        }
        else if(ai > ii)
        {
            nameRange = new Range(start, ii);
            indentRange = new Range(ii + 1, fi);
            formatRange = new Range(fi + 1, ai);
            alignRange = new Range(ai + 1, end);
        }
        else
        {
            nameRange = new Range(start, ai);
            alignRange = new Range(ai + 1, fi);
            formatRange = new Range(fi + 1, ii);
            indentRange = new Range(ii + 1, end);
        }

        var name = text[nameRange.Start..nameRange.End];

        if(name.IsEmpty || TryVerifyPropertyName(ref name) == false)
        {
            return false;
        }

        ReadOnlySpan<char> format = default;
        if(formatRange.IsValid())
        {
            format = text[formatRange.Start..formatRange.End];

            if(TryVerifyFormat(ref format) == false)
            {
                return false;
            }
        }

        sbyte align = 0;
        if(alignRange.IsValid())
        {
            var alignText = text[alignRange.Start..alignRange.End];

            if(TryParseAlign(ref alignText, ref align) == false)
            {
                return false;
            }
        }

        byte indent = 0;
        if(indentRange.IsValid())
        {
            var indentText = text[indentRange.Start..indentRange.End];

            if(TryPraseIndent(ref indentText, ref indent) == false)
            {
                return false;
            }
        }

        if(sbyte.TryParse(name, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number)
            && number >= 0)
        {
            token = TemplateHolder.CreateIndexer(indent, align, number, type, position, format.ToString());
        }
        else
        {
            token = TemplateHolder.CreateNamed(indent, align, type, position, name.ToString(), format.ToString());
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryVerifyPropertyName(ref ReadOnlySpan<char> text)
    {
        if(text.Length > NameLengthLimit)
        {
            return false;
        }

        var index = text.IndexOfAnyExcept(PropertyNameValidChars);

        return index == -1;
    }

    private static bool TryVerifyFormat(ref ReadOnlySpan<char> text)
    {
        if(text.Length > FormatLengthLimit)
        {
            return false;
        }

        var index = 0;
        do
        {
            ref readonly var c = ref text[index];

            if(char.IsLetterOrDigit(c) // 字母或数字
                || char.IsPunctuation(c) // 标点符号
                || char.IsWhiteSpace(c) // 空格
                || char.IsSymbol(c)) // 字符
            {
                continue;
            }

            return false;

        } while(index++ < text.Length - 1);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryPraseIndent(ref ReadOnlySpan<char> text, ref byte indent)
    {
        if(text.Length > 2)
        {
            return false;
        }

        if(text.IndexOfAnyExcept(IndentStringValidChars) == -1)
        {
            var number = byte.Parse(text);

            if(number > IndexNumericLimit)
            {
                return false;
            }

            indent = number;
            return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryParseAlign(ref ReadOnlySpan<char> text, ref sbyte align)
    {
        if(text.Length > 3)
        {
            return false;
        }

        if(text.IndexOfAnyExcept(AlignStringValidChars) == -1)
        {
            var number = sbyte.Parse(text);

            if(number > AlignNumericLimit)
            {
                return false;
            }

            align = number;

            return true;
        }

        return false;
    }

    public bool Equals(TemplateEnumerator other)
    {
        return Template.Equals(other.Template, StringComparison.InvariantCulture);
    }

    public override bool Equals(object? obj)
    {
        return obj is TemplateEnumerator other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Template.GetHashCode();
    }
}
