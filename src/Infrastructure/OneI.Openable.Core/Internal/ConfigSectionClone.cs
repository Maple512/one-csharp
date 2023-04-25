namespace OneI.Openable.Internal;

using Microsoft.Extensions.Configuration;

internal sealed class ConfigSectionClone : IEquatable<ConfigSectionClone>
{
    public ConfigSectionClone(IConfigurationSection section)
    {
        Value = section.Value;

        var children = section.GetChildren()
            ?? Enumerable.Empty<IConfigurationSection>();

        Children = children.ToDictionary(
            x => x.Key,
            x => new ConfigSectionClone(x));
    }

    public string? Value { get; }

    public Dictionary<string, ConfigSectionClone> Children { get; }

    public override bool Equals(object? obj)
    {
        return obj is ConfigSectionClone other
            && Equals(other);
    }

    public bool Equals(ConfigSectionClone? other)
    {
        if(other is null)
        {
            return false;
        }

        if(Value != other.Value
            || Children.Count != other.Children.Count)
        {
            return false;
        }

        var enumerator = Children.GetEnumerator();

        while(enumerator.MoveNext())
        {
            var child = enumerator.Current;

            if(!other.Children.TryGetValue(child.Key, out var clone))
            {
                return false;
            }

            if(child.Value != clone)
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value, Children);
    }

    public static bool operator ==(ConfigSectionClone? left, ConfigSectionClone? right)
    {
        return left is null
            ? right is null : left.Equals(right);
    }

    public static bool operator !=(ConfigSectionClone? left, ConfigSectionClone? right)
    {
        return !(left == right);
    }
}
