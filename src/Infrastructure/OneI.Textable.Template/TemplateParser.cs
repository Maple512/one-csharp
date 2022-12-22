namespace OneI.Textable;

using System.Collections.Generic;
using System.Globalization;
using OneI.Textable.Rendering;
using OneI.Textable.Templating;

using static OneI.Textable.TemplateConstants.Formatters;
/// <summary>
/// The template parser.
/// </summary>

public static class TemplateParser
{
    private static readonly ConcurrentDictionary<CalledLocation, TemplateContext> _cache = new();

    /// <summary>
    /// Parses the.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="file">The file.</param>
    /// <param name="member">The member.</param>
    /// <param name="line">The line.</param>
    /// <returns>A TemplateContext.</returns>
    public static TemplateContext Parse(
        string text,
        [CallerFilePath] string? file = null,
        [CallerMemberName] string? member = null,
        [CallerLineNumber] int? line = null)
    {
        var location = new CalledLocation(file, member, line);

        if(_cache.TryGetValue(location, out var context))
        {
            return context;
        }

        context = _cache.Values.FirstOrDefault(x => x.Text.Equals(x));

        if(context != null)
        {
            return context;
        }

        List<Token> tokens;
        if(text.IsNullOrEmpty())
        {
            tokens = new List<Token>(0);
        }
        else
        {
            tokens = ParseCore(text);
        }

        context = new TemplateContext(text, tokens);

        _cache.TryAdd(location, context);

        return context;
    }

    /// <summary>
    /// Parses the core.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>A list of Tokens.</returns>
    private static List<Token> ParseCore(scoped ReadOnlySpan<char> text)
    {
        var result = new List<Token>();

        var index = 0;
        var lastIndex = text.Length - 1;
        Span<char> span = stackalloc char[text.Length];

        var openBrace = -1;
        var closeBrace = -1;
        var propertyIndex = 0;
        var propertyTotalLength = 0;
        do
        {
            var c = text[index];

            span[index] = c;

            if(c is Open_Separator)
            {
                openBrace = index;
            }
            else if(c == Close_Separator && openBrace > -1)
            {
                if(index != openBrace + 1)
                {
                    var length = index - openBrace - 1;
                    var content = span.Slice(openBrace, length + 2);

                    if(TryParsePropertyToken(
                        content,
                        propertyIndex++,
                        out var propertyToken))
                    {
                        var textLength = index - 2 - length - closeBrace;

                        if(textLength > 0)
                        {
                            var textContent = span.Slice(closeBrace + 1, textLength);

                            result.Add(new TextToken(closeBrace + 1 - propertyTotalLength, textContent.ToString()));
                        }

                        propertyToken!.ResetPosition(index - propertyTotalLength - length - 1);

                        result.Add(propertyToken);

                        closeBrace = index;

                        propertyTotalLength += length + 1;

                        openBrace = -1;

                        continue;
                    }
                }
                else
                {
                    openBrace = -1;
                }
            }

            if(index == lastIndex
                && closeBrace != index)
            {
                var textContent = span.Slice(closeBrace + 1, index - closeBrace);

                result.Add(new TextToken(closeBrace + 1 - propertyTotalLength, textContent.ToString()));
            }
        } while(++index < lastIndex + 1);

        return result;
    }

    /// <summary>
    /// Tries the parse property token.
    /// </summary>
    /// <param name="bytes">The bytes.</param>
    /// <param name="index">The index.</param>
    /// <param name="token">The token.</param>
    /// <returns>A bool.</returns>
    private static bool TryParsePropertyToken(ReadOnlySpan<char> bytes, int index, out PropertyToken? token)
    {
        token = null;

        if(bytes.IsEmpty || bytes.IsWhiteSpace())
        {
            return false;
        }

        string? name = null;
        string? format = null;
        Alignment? align = null;
        int? indent = null;
        Span<char> container = stackalloc char[bytes.Length - 2];
        var start = 0;
        int end;
        var flag = 0;
        for(end = 0; end < bytes.Length - 2; end++)
        {
            var c = bytes[end + 1];
            if(c == Format_Separator && flag != 1)
            {
                if(TryAssignment(container) == false)
                {
                    return false;
                }

                container.Clear();
                start = end;
                flag = 1;
            }
            else if(c == Align_Separator && flag != 2)
            {
                if(TryAssignment(container) == false)
                {
                    return false;
                }

                container.Clear();
                start = end;
                flag = 2;
            }
            else if(c == Indent_Separator && flag != 3)
            {
                if(TryAssignment(container) == false)
                {
                    return false;
                }

                container.Clear();
                start = end;
                flag = 3;
            }
            else
            {
                container[end] = c;
            }
        }

        if(TryAssignment(container) == false)
        {
            return false;
        }

        if(name.IsNullOrWhiteSpace())
        {
            return false;
        }

        token = new PropertyToken(name!, bytes[1..^1].ToString(), index, -1, format, align, indent);

        return true;

        bool TryAssignment(Span<char> text)
        {
            switch(flag)
            {
                case 1:
                    if(TryValidFormat(text[(start + 1)..end], out format) == false)
                    {
                        return false;
                    }

                    break;
                case 2:
                    if(TryValidAlign(text[(start + 1)..end], out align) == false)
                    {
                        return false;
                    }

                    break;
                case 3:
                    if(TryValidIndent(text[(start + 1)..end], out indent) == false)
                    {
                        return false;
                    }

                    break;
                default:
                    if(TryValidPropertyName(text[start..end], out name) == false)
                    {
                        return false;
                    }

                    break;
            }

            return true;
        }
    }

    /// <summary>
    /// Tries the valid property name.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="name">The name.</param>
    /// <returns>A bool.</returns>
    private static bool TryValidPropertyName(ReadOnlySpan<char> text, [NotNullWhen(true)] out string? name)
    {
        name = null;

        for(var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            if(char.IsLetterOrDigit(c) == false
                && c != '_')
            {
                return false;
            }
        }

        name = text.ToString();

        return true;
    }

    /// <summary>
    /// Tries the valid indent.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="indent">The indent.</param>
    /// <returns>A bool.</returns>
    private static bool TryValidIndent(ReadOnlySpan<char> text, out int? indent)
    {
        indent = 0;
        if(text.IsEmpty || text.IsWhiteSpace())
        {
            return false;
        }

        int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var result);

        indent = result;

        return indent != 0;
    }

    /// <summary>
    /// Tries the valid align.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="align">The align.</param>
    /// <returns>A bool.</returns>
    private static bool TryValidAlign(ReadOnlySpan<char> text, [NotNullWhen(true)] out Alignment? align)
    {
        align = null;
        if(text.IsEmpty || text.IsWhiteSpace())
        {
            return false;
        }

        if(int.TryParse(text, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var width))
        {
            align = new Alignment(width);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Tries the valid format.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="format">The format.</param>
    /// <returns>A bool.</returns>
    private static bool TryValidFormat(ReadOnlySpan<char> text, [NotNullWhen(true)] out string? format)
    {
        format = null;
        if(text.IsEmpty || text.IsWhiteSpace())
        {
            return false;
        }

        foreach(var item in text)
        {
            if(IsValidFormat(item) == false)
            {
                return false;
            }
        }

        format = text.ToString();

        return true;
    }

    /// <summary>
    /// Are the valid format.
    /// </summary>
    /// <param name="c">The c.</param>
    /// <returns>A bool.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsValidFormat(char c)
    {
        return char.IsLetterOrDigit(c) // 字母或数字
            || char.IsPunctuation(c) // 标点符号
            || char.IsWhiteSpace(c) // 空格
            || char.IsSymbol(c); // 字符
    }
}
