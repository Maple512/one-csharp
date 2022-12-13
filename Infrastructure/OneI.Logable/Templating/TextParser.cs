namespace OneI.Logable.Templating;

using System;
using System.Globalization;
using OneI.Logable.Templating.Rendering;

public static class TextParser
{
    public static TextTemplate Parse(string text)
    {
        var tokens = text.IsNullOrEmpty()
            ? Array.Empty<Token>()
         : ParseCore(text.AsSpan());

        return new TextTemplate(text, tokens);
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

    // {Date:yyyy-MM-dd,20}
    private static bool TryParsePropertyToken(ReadOnlySpan<char> bytes, int index, out PropertyToken? token)
    {
        token = null;

        if(bytes.IsEmpty || bytes.IsWhiteSpace())
        {
            return false;
        }

        const int startIndex = 1;
        var end = bytes.Length - 1;

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
            name = bytes.Slice(startIndex, end - startIndex);
        }// {Date:yyyy}
        else if(alignIndex == -1
            && formatIndex != -1)
        {
            name = bytes.Slice(startIndex, formatIndex - startIndex);// [startIndex..formatIndex];
            formatChars = bytes.Slice(formatIndex + 1, end - (formatIndex + 1));// [(formatIndex + 1)..^1];
        }// {Data,12}
        else if(formatIndex == -1
            && alignIndex != -1)
        {
            var align = bytes.Slice(alignIndex + 1, end - (alignIndex + 1));//[(alignIndex + 1)..^1];

            if(align.IsEmpty == false
                    && int.TryParse(align.ToString(), NumberStyles.AllowLeadingSign, System.Globalization.CultureInfo.InvariantCulture, out var width))
            {
                alignment = new Alignment(width);
            }
            else
            {
                return false;
            }

            name = bytes.Slice(startIndex, alignIndex - 1);//[startIndex..alignIndex];
        }
        else
        {   // {Data:yyyy,12}
            if(alignIndex > formatIndex)
            {
                var align = bytes.Slice(alignIndex + 1, end - (alignIndex + 1));// [(alignIndex + 1)..^1];

                if(align.IsEmpty == false
                    && int.TryParse(align.ToString(), NumberStyles.AllowLeadingSign, System.Globalization.CultureInfo.InvariantCulture, out var width))
                {
                    alignment = new Alignment(width);
                }
                else
                {
                    return false;
                }

                formatChars = bytes.Slice(formatIndex + 1, alignIndex - (formatIndex + 1));
                name = bytes.Slice(startIndex, formatIndex - startIndex);
            }
            else // {Data,12:yyyy}
            {
                // var align = bytes[(alignIndex + 1)..formatIndex];
                var align = bytes.Slice(alignIndex + 1, formatIndex - (alignIndex + 1));

                if(align.IsEmpty == false
                    && int.TryParse(align.ToString(), NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite, System.Globalization.CultureInfo.InvariantCulture, out var width))
                {
                    alignment = new Alignment(width);
                }
                else
                {
                    return false;
                }

                formatChars = bytes.Slice(formatIndex + 1, end - (formatIndex + 1));
                name = bytes.Slice(startIndex, alignIndex - startIndex);
            }
        }

        if(ValidPropertyName(name) == false)
        {
            return false;
        }

        if(TryCheckFormat(formatChars, out var format) == false)
        {
            return false;
        }

        token = new PropertyToken(name.ToString(), bytes.ToString(), index, -1, format, alignment);

        return true;
    }

    private static bool ValidPropertyName(ReadOnlySpan<char> name)
    {
        for(var i = 0; i < name.Length; i++)
        {
            var c = name[i];
            if(char.IsLetterOrDigit(c) == false
                && c != '_')
            {
                return false;
            }
        }

        return true;
    }

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

        format = chars.ToString();

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsValidFormat(char c) => char.IsLetterOrDigit(c) || char.IsPunctuation(c);
}
