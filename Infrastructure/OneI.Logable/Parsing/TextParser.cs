namespace OneI.Logable.Parsing;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

public static class TextParser
{
    private const byte _open_brace = (byte)'{';
    private const byte _close_brace = (byte)'}';
    private const byte _sign = (byte)'@';
    private const byte _colon = (byte)':';
    private const byte _dollar = (byte)'$';

    public static IReadOnlyList<Token> Parse(string? text)
    {
        if(text.IsNullOrEmpty())
        {
            return Array.Empty<Token>();
        }

        return Parse(Encoding.UTF8.GetBytes(text));
    }

    private static IReadOnlyList<Token> Parse(ReadOnlySpan<byte> text)
    {
        if(text.IsEmpty)
        {
            // TODO: 目前的单元测试对yield有问题，无法进行测试，等待修复 https://github.com/microsoft/vstest/issues/2170
            //yield return Token.Empty;
            //yield break;
            return Array.Empty<Token>();
        }

        var result = new List<Token>();

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

                            result.Add(new TextToken(Encoding.UTF8.GetString(textContent), closeBrace + 1 - propertyTotalLength));
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

                result.Add(new TextToken(Encoding.UTF8.GetString(textContent), closeBrace + 1 - propertyTotalLength));
            }
        } while(++index < lastIndex + 1);

        return result;
    }

    private static bool TryParsePropertyToken(ReadOnlySpan<byte> bytes, int index, [NotNullWhen(true)] out PropertyToken? token)
    {
        var text = Encoding.UTF8.GetString(bytes);

        token = null;

        if(text.IsNullOrWhiteSpace())
        {
            return false;
        }

        var colonIndex = bytes.IndexOf(_colon);
        if(colonIndex == 0)
        {
            return false;
        }

        var hasColon = colonIndex > 0;

        var parsingType = PropertyTokenType.Stringify;
        ReadOnlySpan<byte> name;
        var length = bytes.Length - 2;
        switch(bytes[1])
        {
            case _sign:
                parsingType = PropertyTokenType.Deconstruct;
                name = bytes.Slice(2, hasColon ? colonIndex - 2 : length - 1);
                break;
            case _dollar:
                name = bytes.Slice(2, hasColon ? colonIndex - 2 : length - 1);
                break;
            default:
                name = bytes.Slice(1, hasColon ? colonIndex - 1 : length);
                break;
        }

        var a = Encoding.UTF8.GetString(name);

        if(name.IsEmpty)
        {
            return false;
        }

        string? format = null;

        if(colonIndex > 0)
        {
            format = text[(colonIndex + 1)..^1];
        }

        token = new PropertyToken(Encoding.UTF8.GetString(name), index, text, -1, format, null, parsingType);

        return true;
    }
}
