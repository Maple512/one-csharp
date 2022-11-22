namespace OneI.Logable.Properties.Policies;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using OneI.Logable.Properties.PropertyValues;

public sealed class ByteArrayScalarConversionPolicy : IScalarConversionPolicy
{
    public const int _maxByteArrayLength = 1024;

    private readonly int _maxLength;

    public ByteArrayScalarConversionPolicy(int maxLength = _maxByteArrayLength)
    {
        _maxLength = maxLength;
    }

    public bool TryConvert(object value, [NotNullWhen(true)] out ScalarPropertyValue? propertyValue)
    {
        propertyValue = value switch
        {
            byte[] bytes => Convert(bytes, _maxLength),
            Memory<byte> rs => Convert(rs.ToArray(), _maxLength),
            ReadOnlyMemory<byte> rm => Convert(rm.ToArray(), _maxLength),
            _ => null,
        };

        return propertyValue != null;
    }

    private static ScalarPropertyValue Convert(byte[] bytes, int maxLength)
    {
        var chars = bytes[0..Math.Min(maxLength, bytes.Length)].Select(x => x.ToString("X2"));

        var text = string.Concat(chars, $"... ({bytes.Length} bytes)");

        return new ScalarPropertyValue(text);
    }
}
