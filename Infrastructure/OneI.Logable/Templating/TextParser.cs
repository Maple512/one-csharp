namespace OneI.Logable.Templating;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using OneI.Logable.Templating.Rendering;

public static class TextParser
{
    public static IList<Token> Parse(string? text)
    {
        if(string.IsNullOrEmpty(text))
        {
            return Array.Empty<Token>();
        }

        return ParseCore(text.AsSpan());
    }

    private static IList<Token> ParseCore(ReadOnlySpan<char> text)
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

            if(c is LoggerConstants.Formatters.Open_Separator)
            {
                openBrace = index;
            }
            else if(c == LoggerConstants.Formatters.Close_Separator && openBrace > -1)
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

                            result.Add(new TextToken(closeBrace + 1 - propertyTotalLength, new(textContent)));
                        }

                        propertyToken.ResetPosition(index - propertyTotalLength - length - 1);

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

                result.Add(new TextToken(closeBrace + 1 - propertyTotalLength, new(textContent)));
            }
        } while(++index < lastIndex + 1);

        return result;
    }

    // {Date:yyyy-MM-dd,20}
    private static bool TryParsePropertyToken(ReadOnlySpan<char> bytes, int index, [NotNullWhen(true)] out PropertyToken? token)
    {
        token = null;

        if(bytes.IsEmpty || bytes.IsWhiteSpace())
        {
            return false;
        }

        var renderType = GetPropertyRenderType(bytes[0]);
        var startIndex = renderType.HasValue ? 2 : 1;

        var formatIndex = bytes.IndexOf(LoggerConstants.Formatters.Format_Separator);
        if(formatIndex == startIndex)
        {
            return false;
        }

        var alignIndex = bytes.IndexOf(LoggerConstants.Formatters.Align_Separator);
        if(alignIndex == startIndex)
        {
            return false;
        }

        ReadOnlySpan<char> name = null;
        ReadOnlySpan<char> formatChars = null;
        Alignment? alignment = null;

        // {Date}
        if(formatIndex == -1
            && alignIndex == -1)
        {
            name = bytes[startIndex..^1];
        }// {Date:yyyy}
        else if(alignIndex == -1
            && formatIndex != -1)
        {
            name = bytes[startIndex..formatIndex];
            formatChars = bytes[(formatIndex + 1)..^1];
        }// {Data,12}
        else if(formatIndex == -1
            && alignIndex != -1)
        {
            var align = bytes[(alignIndex + 1)..^1];

            if(align.IsEmpty == false
                    && int.TryParse(align, NumberStyles.AllowLeadingSign, System.Globalization.CultureInfo.InvariantCulture, out var width))
            {
                alignment = new Alignment(width);
            }
            else
            {
                return false;
            }

            name = bytes[startIndex..alignIndex];
        }
        else
        {   // {Data:yyyy,12}
            if(alignIndex > formatIndex)
            {
                var align = bytes[(alignIndex + 1)..^1];

                if(align.IsEmpty == false
                    && int.TryParse(align, NumberStyles.AllowLeadingSign, System.Globalization.CultureInfo.InvariantCulture, out var width))
                {
                    alignment = new Alignment(width);
                }
                else
                {
                    return false;
                }

                formatChars = new string(bytes[(formatIndex + 1)..alignIndex]);
                name = bytes[startIndex..formatIndex];
            }
            else // {Data,12:yyyy}
            {
                var align = bytes[(alignIndex + 1)..formatIndex];

                if(align.IsEmpty == false
                    && int.TryParse(align, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite, System.Globalization.CultureInfo.InvariantCulture, out var width))
                {
                    alignment = new Alignment(width);
                }
                else
                {
                    return false;
                }

                formatChars = bytes[(formatIndex + 1)..^1];
                name = bytes[startIndex..alignIndex];
            }
        }

        if(TryCheckFormat(formatChars, out var format) == false)
        {
            return false;
        }

        token = new PropertyToken(new(name), new(bytes), index, -1, format, alignment, renderType);

        return true;
    }

    private static PropertyRenderType? GetPropertyRenderType(char b) => b switch
    {
        LoggerConstants.Formatters.Render_As_String => PropertyRenderType.Stringification,
        LoggerConstants.Formatters.Render_As_Structure => PropertyRenderType.Structuration,
        _ => null,
    };

    private static bool TryCheckFormat(ReadOnlySpan<char> chars, out string? format)
    {
        format = null;

        if(chars is { IsEmpty: true })
        {
            return true;
        }

        foreach(var item in chars)
        {
            if(IsValidFormat(item) == false)
            {
                return false;
            }
        }

        format = new string(chars);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsValidFormat(char c) => char.IsLetterOrDigit(c) || char.IsPunctuation(c);
}
