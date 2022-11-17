namespace OneI.Logable.Parsing;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

public static class TextParser
{
    private const byte _open_brace = 0x7B; // {
    private const byte _close_brace = 0x7D; // }
    private const char _sign = '@';
    private const char _colon = ':';
    private const char _dollar = '$';

    public static IEnumerable<TextToken> Parse(string text)
    {
        return Parse(Encoding.UTF8.GetBytes(text));
    }

    public static IEnumerable<TextToken> Parse(ReadOnlySpan<byte> text)
    {
        if(text.IsEmpty)
        {
            // TODO: 目前的单元测试对yield有问题，无法进行测试，等待修复 https://github.com/microsoft/vstest/issues/2170
            //yield return Token.Empty;
            //yield break;
            return new TextToken[] { TextToken.Empty };
        }

        var result = new List<TextToken>();

        var index = 0;
        var lastIndex = text.Length - 1;
        Span<byte> span = stackalloc byte[text.Length];

        var openBrace = -1;
        var closeBrace = -1;
        var propertyIndex = 0;
        var propertyTotalLength = 0;
        do
        {
            var c = text[index];

            span[index] = c;

            if(c is _open_brace)
            {
                openBrace = index;
            }
            else if(c == _close_brace && openBrace > -1)
            {
                if(index != (openBrace + 1))
                {
                    var length = index - openBrace - 1;
                    var content = span.Slice(openBrace + 1, length);

                    if(TryParsePropertyToken(
                        Encoding.UTF8.GetString(content),
                        propertyIndex++,
                        out var propertyToken))
                    {
                        var textLength = index - 2 - length - closeBrace;

                        if(textLength > 0)
                        {
                            var textC = span.Slice(closeBrace + 1, textLength);

                            result.Add(new TextToken(Encoding.UTF8.GetString(textC), closeBrace + 1 - propertyTotalLength));
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
                var textC = span.Slice(closeBrace + 1, index - closeBrace);

                result.Add(new TextToken(Encoding.UTF8.GetString(textC), closeBrace + 1 - propertyTotalLength));
            }
        } while(++index < lastIndex + 1);

        return result;
    }

    private static bool TryParsePropertyToken(string text, int index, [NotNullWhen(true)] out PropertyToken? token)
    {
        token = null;

        if(text.IsNullOrWhiteSpace())
        {
            return false;
        }

        var splits = text.Split(_colon);

        if(splits.Length > 2)
        {
            return false;
        }

        var parsingType = PropertyTokenType.Stringify;
        string name;
        switch(text[0])
        {
            case _sign:
                name = splits[0][1..];
                parsingType = PropertyTokenType.Serialization;
                break;
            case _dollar:
                name = splits[0][1..];
                break;
            default:
                name = splits[0];
                break;
        }

        if(name.IsNullOrWhiteSpace())
        {
            return false;
        }

        string? format = null;

        if(splits.Length == 2)
        {
            format = splits[1];
        }

        token = new PropertyToken(name, index, text, -1, format, null, parsingType);

        return true;
    }
}
