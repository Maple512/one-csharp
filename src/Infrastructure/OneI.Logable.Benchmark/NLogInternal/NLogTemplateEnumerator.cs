namespace OneI.Logable.NLogInternal;

using NLog.MessageTemplates;

/// <summary>
/// A hole that will be replaced with a value
/// </summary>
internal readonly struct Hole
{
    /// <summary>
    /// Constructor
    /// </summary>
    public Hole(string name, string format, CaptureType captureType, short parameterIndex, short alignment)
    {
        Name = name;
        Format = format;
        CaptureType = captureType;
        Index = parameterIndex;
        Alignment = alignment;
    }

    /// <summary>Parameter name sent to structured loggers.</summary>
    /// <remarks>This is everything between "{" and the first of ",:}". 
    /// Including surrounding spaces and names that are numbers.</remarks>
    public readonly string Name;
    /// <summary>Format to render the parameter.</summary>
    /// <remarks>This is everything between ":" and the first unescaped "}"</remarks>
    public readonly string Format;
    /// <summary>
    /// Type
    /// </summary>
    public readonly CaptureType CaptureType;
    /// <summary>When the template is positional, this is the parsed name of this parameter.</summary>
    /// <remarks>For named templates, the value of Index is undefined.</remarks>
    public readonly short Index;
    /// <summary>Align to render the parameter, by default 0.</summary>
    /// <remarks>This is the parsed value between "," and the first of ":}"</remarks>
    public readonly short Alignment;
}

internal readonly struct Literal
{
    /// <summary>Number of characters from the original template to copy at the current position.</summary>
    /// <remarks>This can be 0 when the template starts with a hole or when there are multiple consecutive holes.</remarks>
    public readonly int Print;
    /// <summary>Number of characters to skip in the original template at the current position.</summary>
    /// <remarks>0 is a special value that mean: 1 escaped char, no hole. It can also happen last when the template ends with a literal.</remarks>
    public readonly int Skip;

    public Literal(int print, int skip)
    {
        Print = print;
        Skip = skip;
    }
}

internal struct LiteralHole
{
    /// <summary>Literal</summary>
    public Literal Literal; // Not readonly to avoid struct-copy, and to avoid VerificationException when medium-trust AppDomain
    /// <summary>Hole</summary>
    /// <remarks>Uninitialized when <see cref="MessageTemplates.Literal.Skip"/> = 0.</remarks>
    public Hole Hole;       // Not readonly to avoid struct-copy, and to avoid VerificationException when medium-trust AppDomain

    public LiteralHole(Literal literal, Hole hole)
    {
        Literal = literal;
        Hole = hole;
    }

    public bool MaybePositionalTemplate => Literal.Skip != 0 && Hole.Index != -1 && Hole.CaptureType == CaptureType.Normal;
}
internal struct NLogTemplateEnumerator : IEnumerator<LiteralHole>
{
    private static readonly char[] HoleDelimiters = { '}', ':', ',' };
    private static readonly char[] TextDelimiters = { '{', '}' };

    private string _template;
    private int _length;
    private int _position;
    private int _literalLength;
    private LiteralHole _current;
    private const short Zero = 0;

    /// <summary>
    /// Parse a template.
    /// </summary>
    /// <param name="template">Template to be parsed.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="template"/> is null.</exception>
    /// <returns>Template, never null</returns>
    public NLogTemplateEnumerator(string template)
    {
        _template = template ?? throw new ArgumentNullException(nameof(template));
        _length = _template.Length;
        _position = 0;
        _literalLength = 0;
        _current = default;
    }

    /// <summary>
    /// Gets the current literal/hole in the template
    /// </summary>
    public LiteralHole Current => _current;

    object System.Collections.IEnumerator.Current => _current;

    /// <summary>
    /// Clears the enumerator
    /// </summary>
    public void Dispose()
    {
        _template = string.Empty;
        _length = 0;
        Reset();
    }

    /// <summary>
    /// Restarts the enumerator of the template
    /// </summary>
    public void Reset()
    {
        _position = 0;
        _literalLength = 0;
        _current = default;
    }

    /// <summary>
    /// Moves to the next literal/hole in the template
    /// </summary>
    /// <returns>Found new element [true/false]</returns>
    public bool MoveNext()
    {
        try
        {
            while(_position < _length)
            {
                var c = Peek();
                if(c == '{')
                {
                    ParseOpenBracketPart();
                    return true;
                }
                else if(c == '}')
                {
                    ParseCloseBracketPart();
                    return true;
                }
                else
                {
                    ParseTextPart();
                }
            }

            if(_literalLength != 0)
            {
                AddLiteral();
                return true;
            }

            return false;
        }
        catch(IndexOutOfRangeException)
        {
            throw new TemplateParserException("Unexpected end of template.", _position, _template);
        }
    }

    private void AddLiteral()
    {
        _current = new LiteralHole(new Literal(_literalLength, Zero), default);
        _literalLength = 0;
    }

    private void ParseTextPart()
    {
        _literalLength = SkipUntil(TextDelimiters, required: false);
    }

    private void ParseOpenBracketPart()
    {
        Skip('{');
        var c = Peek();
        switch(c)
        {
            case '{':
                Skip('{');
                _literalLength++;
                AddLiteral();
                return;
            case '@':
                Skip('@');
                ParseHole(CaptureType.Serialize);
                return;
            case '$':
                Skip('$');
                ParseHole(CaptureType.Stringify);
                return;
            default:
                ParseHole(CaptureType.Normal);
                return;
        }
    }

    private void ParseCloseBracketPart()
    {
        Skip('}');
        if(Read() != '}')
        {
            throw new TemplateParserException("Unexpected '}}' ", _position - 2, _template);
        }

        _literalLength++;
        AddLiteral();
    }

    private void ParseHole(CaptureType type)
    {
        var start = _position;
        var name = ParseName(out var parameterIndex);
        var alignment = 0;
        string? format = null;
        if(Peek() != '}')
        {
            alignment = Peek() == ',' ? ParseAlignment() : 0;
            format = Peek() == ':' ? ParseFormat() : null;
            Skip('}');
        }
        else
        {
            _position++;
        }

        var literalSkip = _position - start + (type == CaptureType.Normal ? 1 : 2);     // Account for skipped '{', '{$' or '{@'
        _current = new LiteralHole(new Literal(_literalLength, literalSkip), new Hole(
            name,
            format,
            type,
            (short)parameterIndex,
            (short)alignment
        ));
        _literalLength = 0;
    }

    private string ParseName(out int parameterIndex)
    {
        parameterIndex = -1;

        var c = Peek();
        // If the name matches /^\d+ *$/ we consider it positional
        if(c is >= '0' and <= '9')
        {
            var start = _position;
            var parsedIndex = ReadInt();
            c = Peek();

            if(c is '}' or ':' or ',')
            {
                // Non-allocating positional hole-name-parsing
                parameterIndex = parsedIndex;
                return ParameterIndexToString(parameterIndex);
            }

            if(c == ' ')
            {
                SkipSpaces();
                c = Peek();
                if(c is '}' or ':' or ',')
                {
                    parameterIndex = parsedIndex;
                }
            }

            _position = start;
        }

        return ReadUntil(HoleDelimiters);
    }

    private static string ParameterIndexToString(int parameterIndex)
    {
        return parameterIndex switch
        {
            0 => "0",
            1 => "1",
            2 => "2",
            3 => "3",
            4 => "4",
            5 => "5",
            6 => "6",
            7 => "7",
            8 => "8",
            9 => "9",
            _ => parameterIndex.ToString(System.Globalization.CultureInfo.InvariantCulture),
        };
    }

    /// <summary>
    /// Parse format after hole name/index. Handle the escaped { and } in the format. Don't read the last }
    /// </summary>
    /// <returns></returns>
    private string ParseFormat()
    {

        Skip(':');
        var format = ReadUntil(TextDelimiters);
        while(true)
        {
            var c = Read();

            switch(c)
            {
                case '}':
                {
                    if(_position < _length && Peek() == '}')
                    {
                        //this is an escaped } and need to be added to the format.
                        Skip('}');
                        format += "}";
                    }
                    else
                    {
                        //done. unread the '}' .
                        _position--;
                        //done
                        return format;
                    }

                    break;
                }
                case '{':
                {
                    //we need a second {, otherwise this format is wrong.
                    var next = Peek();
                    if(next == '{')
                    {
                        //this is an escaped } and need to be added to the format.
                        Skip('{');
                        format += "{";
                    }
                    else
                    {
                        throw new TemplateParserException($"Expected '{{' but found '{next}' instead in format.",
                            _position, _template);
                    }

                    break;
                }
            }

            format += ReadUntil(TextDelimiters);
        }
    }

    private int ParseAlignment()
    {
        Skip(',');
        SkipSpaces();
        var i = ReadInt();
        SkipSpaces();
        var next = Peek();
        if(next is not ':' and not '}')
        {
            throw new TemplateParserException($"Expected ':' or '}}' but found '{next}' instead.", _position, _template);
        }

        return i;
    }

    private char Peek() => _template[_position];

    private char Read() => _template[_position++];

    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    private void Skip(char c)
    {
        // Can be out of bounds, but never in correct use (expects a required char).
        Debug.Assert(_template[_position] == c);
        _position++;
    }

    private void SkipSpaces()
    {
        // Can be out of bounds, but never in correct use (inside a hole).
        while(_template[_position] == ' ')
        {
            _position++;
        }
    }

    private int SkipUntil(char[] search, bool required = true)
    {
        var start = _position;
        var i = _template.IndexOfAny(search, _position);
        if(i == -1 && required)
        {
            var formattedChars = string.Join(", ", search.Select(c => string.Concat("'", c.ToString(), "'")).ToArray());
            throw new Exception($"Reached end of template while expecting one of {formattedChars}.");
        }

        _position = i == -1 ? _length : i;
        return _position - start;
    }

    private int ReadInt()
    {
        var negative = false;
        var i = 0;
        for(var x = 0; x < 12; ++x)
        {
            var c = Peek();

            if(c is < '0' or > '9')
            {
                if(x > 0 && !negative)
                {
                    return i;
                }

                if(x > 1 && negative)
                {
                    return -i;
                }

                if(x == 0 && c == '-')
                {
                    negative = true;
                    _position++;
                    continue;
                }

                break;
            }

            _position++;
            i = i * 10 + (c - '0');
        }

        throw new TemplateParserException("An integer is expected", _position, _template);
    }

    private string ReadUntil(char[] search, bool required = true)
    {
        var start = _position;
        return _template.Substring(start, SkipUntil(search, required));
    }
}
