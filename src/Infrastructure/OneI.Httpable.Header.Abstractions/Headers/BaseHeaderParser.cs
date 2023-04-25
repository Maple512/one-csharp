namespace OneI.Httpable.Headers;

using Microsoft.Extensions.Primitives;

internal abstract class BaseHeaderParser<T> : HeaderParser<T>
{
    protected BaseHeaderParser(bool supportsMultipleValues) : base(supportsMultipleValues)
    {
    }

    protected abstract int GetParsedValueLength(StringSegment value, int startIndex, out T? parsedValue);

    public sealed override bool TryParseValue(StringSegment value, ref int index, out T? parsedValue) //where T : default
    {
        parsedValue = default;

        // 如果支持多个值（即值列表），则接受一个空字符串：标头可能会多次添加到请求/响应消息中。
        // Accept: text/xml; q=1
        // Accept:
        // Accept: text/plain; q=0.2
        if(StringSegment.IsNullOrEmpty(value)
            || (index == value.Length))
        {
            return SupportsMultipleValues;
        }


        throw new NotImplementedException();
    }
}
