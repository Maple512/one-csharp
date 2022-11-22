namespace OneI.Logable.Properties.Policies;

using System;
using System.Diagnostics.CodeAnalysis;
using OneI.Logable.Properties.PropertyValues;

public class DelegateDestructuringPolicy : IDestructuringPolicy
{
    public bool TryDestructure(
        object value,
        IPropertyValueFactory propertyValueFactory,
        [NotNullWhen(true)] out PropertyValue? propertyValue)
    {
        propertyValue = null;
        if(value is Delegate d)
        {
            propertyValue = new ScalarPropertyValue(d.ToString());
        }

        return propertyValue != null;
    }
}
