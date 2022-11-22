namespace OneI.Logable.Properties.Policies;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using OneI.Logable.Properties.PropertyValues;

public class CurrencyScalarConversionPolicy : IScalarConversionPolicy
{
    private readonly HashSet<Type> _types = new()
    {
        typeof(bool),typeof(char),typeof(sbyte),typeof(byte),typeof(short),typeof(ushort),
        typeof(int),typeof(uint),typeof(long), typeof(ulong),
        typeof(float),typeof(double),typeof(decimal),
        typeof(string),typeof(DateTime),typeof(DateTimeOffset),
        typeof(TimeOnly),typeof(DateOnly)
    };

    public CurrencyScalarConversionPolicy(IEnumerable<Type> types)
    {
        _types.AddAll(types);
    }

    public bool TryConvert(object value, [NotNullWhen(true)] out ScalarPropertyValue? propertyValue)
    {
        if(_types.Contains(value.GetType()))
        {
            propertyValue = new ScalarPropertyValue(value);
            return true;
        }

        propertyValue = null;
        return false;
    }
}
