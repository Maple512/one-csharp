namespace OneI.Logable.Templates;

using Cysharp.Text;

[Obsolete]
[StructLayout(LayoutKind.Auto)]
public readonly struct PropertyValue : IEquatable<PropertyValue>
{
    public readonly object? Value;

    public PropertyValue(object? value)
    {
        Value = value;
    }

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is PropertyValue pv && Equals(pv);

    // TODO: 这里要检查下，是不是调用原始对象的Equals方法，不能调用object的Equals方法
    public override int GetHashCode() => Value?.GetHashCode() ?? 0;

    public override string ToString()
    {
        return $"[{nameof(Value)}: {Value}, Type: {OneIReflectionExtensions.GetTypeDisplayName(Value, false)}]";
    }

    public bool Equals(PropertyValue other)
    {
        Debugger.Break();
        if(Value is null && other.Value is null)
        {
            return true;
        }

        if(Value is not null && other.Value is not null)
        {
            // TODO: 这里要检查下，是不是调用原始对象的Equals方法，不能调用object的Equals方法
            return Value.Equals(other.Value);
        }

        return false;
    }

    public static bool operator ==(PropertyValue left, PropertyValue right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PropertyValue left, PropertyValue right)
    {
        return !(left == right);
    }
}
