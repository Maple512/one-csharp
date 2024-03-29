namespace OneI.Httpable.Headers;

using System.Diagnostics.Contracts;
using System.Globalization;
using System.Net.Http.Headers;
using Microsoft.Extensions.Primitives;

public sealed class NameValuePair
{
    private static readonly HeaderParser<NameValuePair> SingleValueParser
        = new GenericHeaderParser<NameValuePair>(false, GetNameValueLength);
    internal static readonly HeaderParser<NameValuePair> MultipleValueParser
        = new GenericHeaderParser<NameValuePair>(true, GetNameValueLength);

    private StringSegment _name;
    private StringSegment _value;
    private bool _isReadOnly;

    private NameValuePair() { }

    public NameValuePair(StringSegment name, StringSegment value)
    {
        _name = name;
        _value = value;
    }

    public StringSegment Name
    {
        get { return _name; }
    }

    public StringSegment Value
    {
        get => _value;
        set
        {
            HeaderUtilities.ThrowIfReadOnly(IsReadOnly);

            CheckValueFormat(value);

            _value = value;
        }
    }

    public bool IsReadOnly { get { return _isReadOnly; } }

    internal static int GetValueLength(StringSegment input, int startIndex)
    {
        if(startIndex >= input.Length)
        {
            return 0;
        }

        var valueLength = HttpRuleParser.GetTokenLength(input, startIndex);

        if(valueLength == 0)
        {
            // A value can either be a token or a quoted string. Check if it is a quoted string.
            if(HttpRuleParser.GetQuotedStringLength(input, startIndex, out valueLength) != HttpParseResult.Parsed)
            {
                // We have an invalid value. Reset the name and return.
                return 0;
            }
        }
        return valueLength;
    }

    private static int GetNameValueLength(StringSegment input, int startIndex, out NameValuePair? parsedValue)
    {
        Contract.Requires(startIndex >= 0);

        parsedValue = null;

        if(StringSegment.IsNullOrEmpty(input) || (startIndex >= input.Length))
        {
            return 0;
        }

        // Parse the name, i.e. <name> in name/value string "<name>=<value>". Caller must remove
        // leading whitespaces.
        var nameLength = HttpRuleParser.GetTokenLength(input, startIndex);

        if(nameLength == 0)
        {
            return 0;
        }

        var name = input.Subsegment(startIndex, nameLength);
        var current = startIndex + nameLength;
        current = current + HttpRuleParser.GetWhitespaceLength(input, current);

        // Parse the separator between name and value
        if((current == input.Length) || (input[current] != '='))
        {
            // We only have a name and that's OK. Return.
            parsedValue = new NameValuePair();
            parsedValue._name = name;
            current = current + HttpRuleParser.GetWhitespaceLength(input, current); // skip whitespaces
            return current - startIndex;
        }

        current++; // skip delimiter.
        current = current + HttpRuleParser.GetWhitespaceLength(input, current);

        // Parse the value, i.e. <value> in name/value string "<name>=<value>"
        int valueLength = GetValueLength(input, current);

        // Value after the '=' may be empty
        // Use parameterless ctor to avoid double-parsing of name and value, i.e. skip public ctor validation.
        parsedValue = new NameValuePair();
        parsedValue._name = name;
        parsedValue._value = input.Subsegment(current, valueLength);
        current = current + valueLength;
        current = current + HttpRuleParser.GetWhitespaceLength(input, current); // skip whitespaces
        return current - startIndex;
    }

    public static NameValuePair? Find(List<NameValuePair>? values, StringSegment name)
    {
        if(values.IsNullOrEmpty()) return null;

        for(int i = 0; i < values.Count; i++)
        {
            var value = values[i];

            if(value._name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return value;
            }
        }

        return null;
    }

    private static void CheckValueFormat(StringSegment value)
    {
        // 值为null/空或有效的令牌/带引号的字符串
        if(!(StringSegment.IsNullOrEmpty(value)
            || (GetValueLength(value, 0) == value.Length)))
        {
            throw new FormatException($"The header value is invalid: '{value}'");
        }
    }
}
