namespace OneI.Httpable.Headers;

using Microsoft.Extensions.Primitives;

internal sealed class GenericHeaderParser<T> : BaseHeaderParser<T>
{
    internal delegate int GetParsedValueLengthDelegate(StringSegment value, int startIndex, out T? parsedValue);

    private readonly GetParsedValueLengthDelegate _getParsedValueLength;

    internal GenericHeaderParser(bool supportsMultipleValues, GetParsedValueLengthDelegate getParsedValueLength)
        : base(supportsMultipleValues)
    {
        ArgumentNullException.ThrowIfNull(getParsedValueLength);

        _getParsedValueLength = getParsedValueLength;
    }

    protected override int GetParsedValueLength(StringSegment value, int startIndex, out T? parsedValue)
    {
        return _getParsedValueLength(value, startIndex, out parsedValue);
    }
}
