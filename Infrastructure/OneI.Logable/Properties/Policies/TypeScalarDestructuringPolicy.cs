namespace OneI.Logable.Properties.Policies;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using OneI.Logable.Properties.PropertyValues;

public class TypeScalarDestructuringPolicy : IDestructuringPolicy
{
    public bool TryDestructure(object value, IPropertyValueFactory propertyValueFactory, [NotNullWhen(true)] out PropertyValue? propertyValue)
    {
        propertyValue = null;

        if(value is Type or MemberInfo)
        {
            propertyValue = new ScalarPropertyValue(value);
        }

        return propertyValue != null;
    }
}
