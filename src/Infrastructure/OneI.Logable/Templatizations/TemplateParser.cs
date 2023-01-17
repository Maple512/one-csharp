namespace OneI.Logable.Templatizations;

using System.Globalization;
using OneI.Logable.Templatizations.Tokenizations;
using static TemplateConstants.Formatters;
using static TemplateConstants.Property;

public static class TemplateParser
{
    public static List<ITemplateToken> Parse(scoped in ReadOnlySpan<char> text)
    {

        if(text.IsEmpty)
        {
            return new(0);
        }

        if(text.IsWhiteSpace())
        {
            return new() { new TextToken(0, new string(' ', text.Length)) };
        }

        var result = new List<ITemplateToken>(10);

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

                    result.Insert(result.Count - 1, new TextToken(textStart, textToken.ToString()));
                }

                textStart = index;

            }
        }

        if(textStart < textEnd)
        {
            var textToken = text[textStart..textEnd];

            result.Add(new TextToken(textStart, textToken.ToString()));
        }

        return result;
    }

    private static bool TryParseProperty(
        ref int position,
        ref ReadOnlySpan<char> text,
        ref int index,
       [NotNullWhen(true)] out ITemplateToken? token)
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

        ref readonly var typeChar = ref text[0];
        var type = typeChar switch
        {
            '@' => PropertyTokenType.Serialize,
            '$' => PropertyTokenType.Stringify,
            _ => PropertyTokenType.None,
        };

        var start = type > PropertyTokenType.None ? 1 : 0;

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

        var parameterIndex = default(int?);

        if(int.TryParse(name, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number)
            && number >= 0)
        {
            parameterIndex = number;
        }

        if(parameterIndex != null)
        {
            token = new IndexerPropertyToken(parameterIndex.Value, type, format.ToString(), index, align, indent);
        }
        else
        {
            token = new NamedPropertyToken(name.ToString(), type, format.ToString(), index, align, indent);
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
    private static bool TryPraseIndent(ref ReadOnlySpan<char> text, ref int? indent)
    {
        if(text.Length > 2)
        {
            return false;
        }

        if(text.IndexOfAnyExcept(IndentStringValidChars) == -1)
        {
            var number = int.Parse(text);

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
    private static bool TryParseAlign(ref ReadOnlySpan<char> text, ref int? align)
    {
        if(text.Length > 3)
        {
            return false;
        }

        if(text.IndexOfAnyExcept(AlignStringValidChars) == -1)
        {
            var number = int.Parse(text);

            if(number > IndexNumericLimit)
            {
                return false;
            }

            align = number;

            return true;
        }

        return false;
    }
}
