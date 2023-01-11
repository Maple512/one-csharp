namespace OneI.Logable;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using OneI.Logable.Rendering;
using OneI.Logable.Templating;

using static OneI.Logable.TextTemplateConstants.Formatters;

public static class TemplateParser
{
    public static List<Token> Parse(scoped in ReadOnlySpan<char> text)
    {
        if(text.IsEmpty)
        {
            return new(0);
        }

        if(text.IsWhiteSpace())
        {
            return new() { new TextToken(0, new string(' ', text.Length)) };
        }

        var result = new List<Token>();

        var textStart = 0;
        var textEnd = 0;
        var index = 0;
        var propertyCount = 0;

        while(true)
        {
            if(index >= text.Length)
            {
                break;
            }

            var remainder = text[index..];
            var open = remainder.IndexOf(Open_Separator);
            if(open == -1)
            {
                textEnd = index + remainder.Length;
                break;
            }
            var close = remainder.IndexOf(Close_Separator);
            if(close is -1)
            {
                index++;
                continue;
            }

            if(close - open <= 1)
            {
                index += close + 1;
                continue;
            }

            var start = open + 1;

            var property = remainder[start..close];

            if(TryParseProperty(ref index, ref property, ref propertyCount, out var token))
            {
                textEnd = index + open;
                index += close + 1;

                result.Add(token);

                propertyCount++;

                if(textStart != textEnd)
                {
                    var textToken = text[textStart..textEnd];

                    result.Insert(result.Count - 1, new TextToken(textStart, ref textToken));
                }

                textStart = index;
            }
        }

        if(textStart < textEnd)
        {
            var textToken = text[textStart..textEnd];

            result.Add(new TextToken(textStart, ref textToken));
        }

        return result;
    }

    private static bool TryParseProperty(
        ref int start,
        ref ReadOnlySpan<char> text,
        ref int index,
       [NotNullWhen(true)] out PropertyToken? token)
    {
        token = null;

        if(text.IsEmpty)
        {
            return false;
        }

        var fi = text.IndexOf(Format_Separator);
        var ai = text.IndexOf(Align_Separator);
        var ii = text.IndexOf(Indent_Separator);

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

        var nameRange = default(Range);
        var formatRange = default(Range);
        var alignRange = default(Range);
        var indentRange = default(Range);

        if(ai > fi && ii > fi)
        {
            nameRange = new Range(0, fi);
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
                nameRange = new Range(0, ii);
                alignRange = new Range(ai + 1, fi);
                indentRange = new Range(ii + 1, ai);
            }
            else
            {
                nameRange = new Range(0, ai);
                alignRange = new Range(ai + 1, ii);
                indentRange = new Range(ii + 1, end);
            }
        }
        else
        {
            if(ai > ii)
            {
                nameRange = new Range(0, ii);
                indentRange = new Range(ii + 1, fi);
                formatRange = new Range(fi + 1, ai);
                alignRange = new Range(ai + 1, end);
            }
            else
            {
                nameRange = new Range(0, ai);
                alignRange = new Range(ai + 1, fi);
                formatRange = new Range(fi + 1, ii);
                indentRange = new Range(ii + 1, end);
            }
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

        int? align = null;
        if(alignRange.IsValid())
        {
            var alignText = text[alignRange.Start..alignRange.End];

            if(TryParseAlign(ref alignText, ref align) == false)
            {
                return false;
            }
        }

        int? indent = default;
        if(indentRange.IsValid())
        {
            var indentText = text[indentRange.Start..indentRange.End];

            if(TryPraseIndent(ref indentText, ref indent) == false)
            {
                return false;
            }
        }

        var parmeterIndex = default(int?);

        if(int.TryParse(name, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number)
            && number >= 0)
        {
            parmeterIndex = number;
        }

        token = new PropertyToken(
            ref name,
            ref text,
            index,
            start,
            ref format,
            align,
            indent,
            parmeterIndex);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryVerifyPropertyName(ref ReadOnlySpan<char> text)
    {
        if(text.Length > PropertyToken.NameLengthLimit)
        {
            return false;
        }

        var index = text.IndexOfAnyExcept("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_");

        return index == -1;
    }

    private static bool TryVerifyFormat(ref ReadOnlySpan<char> text)
    {
        if(text.Length > PropertyToken.FormatLengthLimit)
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
    private static bool TryPraseIndent(ref ReadOnlySpan<char> text, ref int? indent)
    {
        if(text.Length > 2)
        {
            return false;
        }

        if(text.IndexOfAnyExcept("0123456789") == -1)
        {
            var number = int.Parse(text);

            if(number > PropertyToken.IndexNumericLimit)
            {
                return false;
            }

            indent = number;
            return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryParseAlign(ref ReadOnlySpan<char> text, ref int? align)
    {
        if(text.Length > 3)
        {
            return false;
        }

        if(text.IndexOfAnyExcept("0123456789-") == -1)
        {
            var number = int.Parse(text);

            if(number > PropertyToken.IndexNumericLimit)
            {
                return false;
            }

            align = number;

            return true;
        }

        return false;
    }
}
