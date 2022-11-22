namespace OneI.Logable.Properties.Policies;

using System.Diagnostics.CodeAnalysis;
using OneI.Logable.Properties.PropertyValues;

public interface IScalarConversionPolicy
{
    bool TryConvert(object value, [NotNullWhen(true)] out ScalarPropertyValue? propertyValue);
}
