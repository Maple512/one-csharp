namespace OneI.Logable.Properties.Policies;

using System;
using System.Diagnostics.CodeAnalysis;

public class CustomDestructuringPolicy : IDestructuringPolicy
{
    private readonly Func<Type, bool> _condition;
    private readonly Func<object, object> _custom;

    public CustomDestructuringPolicy(Func<Type, bool> condition, Func<object, object> custom)
    {
        _condition = condition;
        _custom = custom;
    }

    public bool TryDestructure(
        object value,
        IPropertyValueFactory propertyValueFactory,
        [NotNullWhen(true)] out PropertyValue? propertyValue)
    {
        propertyValue = null;

        if(_condition(value.GetType()))
        {
            var result = _custom(value);

            propertyValue = propertyValueFactory.Create(result, true);
        }

        return propertyValue != null;
    }
}
