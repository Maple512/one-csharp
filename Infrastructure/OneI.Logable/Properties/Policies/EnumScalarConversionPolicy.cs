namespace OneI.Logable.Properties.Policies;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using OneI.Logable.Properties.PropertyValues;

public class EnumScalarConversionPolicy : IScalarConversionPolicy
{
    public bool TryConvert(object value, [NotNullWhen(true)] out ScalarPropertyValue? propertyValue)
    {
        if(value.GetType().GetTypeInfo().IsEnum)
        {
            propertyValue = new ScalarPropertyValue(value);
        }
        else
        {
            propertyValue = null;
        }

        return propertyValue != null;
    }
}
