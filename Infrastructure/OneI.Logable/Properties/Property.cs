namespace OneI.Logable.Properties;

using System;

public readonly struct Property : IEquatable<Property>
{
    public static Property None;

    public Property(string name, PropertyValue value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; }

    public PropertyValue Value { get; }

    public override bool Equals(object? obj)
    {
        return obj is Property p && Equals(p);
    }

    public bool Equals(Property other)
        => string.Equals(other.Name, Name) && Equals(other.Value, Value);

    public override int GetHashCode()
        => HashCode.Combine(Name, Value.GetHashCode());

    public static bool operator ==(Property left, Property right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Property left, Property right)
    {
        return !(left == right);
    }
}
