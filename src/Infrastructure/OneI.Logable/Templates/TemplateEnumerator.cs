namespace OneI.Logable.Templates;
public class TemplateQueue : IEnumerable<TemplateHolder>
{
    public readonly ReadOnlyMemory<char> Text;

    public TemplateQueue(ReadOnlyMemory<char> text) => Text = text;

    public IEnumerator<TemplateHolder> GetEnumerator() => new TemplateEnumerator(Text);

    IEnumerator IEnumerable.GetEnumerator() => new TemplateEnumerator(Text);
}

public struct TemplateEnumerator : IEquatable<TemplateEnumerator>, IEnumerator<TemplateHolder>
{
    private const char _open = '{';
    private const char _close = '}';
    private const char _format = ':';
    private const char _indent = '\'';
    private const char _align = ',';

    public readonly ReadOnlyMemory<char> Text;
    private TemplateHolder? _next;
    private readonly ushort _length;
    private ushort _position;

    public TemplateEnumerator()
    {
        _position = 0;
        _length = 0;
    }

    public TemplateEnumerator(ReadOnlyMemory<char> template)
    {
        if(template is { Length: > ushort.MaxValue, })
        {
            throw new ArgumentOutOfRangeException(nameof(template));
        }

        Text = template;
        Current = default;
        _length = (ushort)template.Length;
        _position = 0;
    }

    public bool MoveNext()
    {
        if(_next is not null)
        {
            Current = _next.Value;
            _next = null;
            return true;
        }

        if(_position >= _length)
        {
            return false;
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

            scoped var remainder = Text.Span.Slice(position, length);

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
                position += (ushort)(closeIndex + 1);
                continue;
            }

            position += (ushort)openIndex;
            var property = remainder[(openIndex + 1)..closeIndex];

            if(TryParseProperty(ref property, out _next))
            {
                if(_position != position)
                {
                    length = (ushort)(position - _position);
                    Current = TemplateHolder.CreateText(Text.Slice(_position, length));
                    _position = (ushort)(position + property.Length + 2);
                    return true;
                }

                Current = _next.Value;
                _next = null;
                _position += (ushort)(closeIndex + 1);
                return true;
            }

            position += (ushort)(closeIndex + 1);
        }

        var len = _length - _position;
        Current = TemplateHolder.CreateText(Text.Slice(_position, len));
        _position = _length;

        return true;
    }

    public TemplateHolder Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        private set;
    }

    object IEnumerator.Current => Current;

    public override string ToString() => Text.ToString();

    private static bool TryParseProperty(
        ref ReadOnlySpan<char> text,
        [NotNullWhen(true)] out TemplateHolder? token)
    {
        token = null;

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
        Range nameRange;
        Range formatRange;
        Range alignRange;
        Range indentRange;
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

        if(name.IsEmpty)
        {
            return false;
        }

        ReadOnlySpan<char> format = default;
        if(formatRange.IsValid())
        {
            format = text[formatRange.Start..formatRange.End];
        }

        sbyte align = 0;
        if(alignRange.IsValid())
        {
            var alignText = text[alignRange.Start..alignRange.End];

            _ = sbyte.TryParse(alignText, out align);
        }

        byte indent = 0;
        if(indentRange.IsValid())
        {
            var indentText = text[indentRange.Start..indentRange.End];

            _ = byte.TryParse(indentText, out indent);
        }

        if(byte.TryParse(name, out var parameterIndex))
        {
            token = TemplateHolder.CreateIndexer(indent, align, (sbyte)parameterIndex, type, format.ToString());
        }
        else
        {
            token = TemplateHolder.CreateNamed(indent, align, type, name.ToString(), format.ToString());
        }

        return true;
    }

    public bool Equals(TemplateEnumerator other) => Text.Equals(other.Text);

    public override bool Equals(object? obj) => obj is TemplateEnumerator other && Equals(other);

    public override int GetHashCode() => Text.GetHashCode();

    public void Reset() => _position = 0;
    public void Dispose() { }

    public static bool operator ==(TemplateEnumerator left, TemplateEnumerator right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TemplateEnumerator left, TemplateEnumerator right)
    {
        return !(left == right);
    }
}
