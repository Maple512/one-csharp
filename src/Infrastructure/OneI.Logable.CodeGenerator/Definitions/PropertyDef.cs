namespace OneI.Logable.Definitions;

public class PropertyDef
{
    public PropertyDef(string name, TypeDef type)
        : this(name, -1, type)
    { }

    public PropertyDef(string name, int index, TypeDef type)
    {
        Name = name;
        Index = index;
        Type = type;
    }

    public string Name { get; }

    public int Index { get; }

    public TypeDef Type { get; }

    public bool Equals(PropertyDef other)
        => other is not null
           && Name.Equals(other.Name, StringComparison.InvariantCulture)
           && Index == other.Index
           && Type.Equals(other.Type);

    public override bool Equals(object obj) => obj is PropertyDef pd && Equals(pd);

    public override int GetHashCode()
    {
        var hashCode = 1280133078;
        hashCode = hashCode * -1521134295 + Name.GetHashCode();
        hashCode = hashCode * -1521134295 + Index.GetHashCode();
        hashCode = hashCode * -1521134295 + Type.GetHashCode();
        return hashCode;
    }

    public override string ToString() => Name;
}
