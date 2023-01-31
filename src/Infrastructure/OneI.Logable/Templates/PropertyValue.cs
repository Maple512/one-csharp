namespace OneI.Logable.Templates;

using System.Diagnostics.CodeAnalysis;

public readonly struct PropertyValue : IEquatable<PropertyValue>
{
    public readonly object? Value;

    public PropertyValue(object? value) => Value = value;

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is PropertyValue pv && Equals(pv);

    // TODO: 这里要检查下，是不是调用原始对象的Equals方法，不能调用object的Equals方法
    public override int GetHashCode() => Value?.GetHashCode() ?? 0;

    public override string ToString()
    {
        return Value?.ToString() ?? string.Empty;
    }

    public bool Equals(PropertyValue other)
    {
        if(Value is null && other.Value is null)
        {
            return true;
        }
        else if(Value is not null && other.Value is not null)
        {
            // TODO: 这里要检查下，是不是调用原始对象的Equals方法，不能调用object的Equals方法
            return Value.Equals(other.Value);
        }

        return false;
    }
}
