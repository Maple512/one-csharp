namespace OneI.Logable.Definitions;

using System.Collections.Generic;

public class ParameterDef : IEquatable<ParameterDef>
{
    public ParameterDef(int index, TypeDef type)
        : this(index, $"arg{index}", type) { }

    public ParameterDef(int index, string name, TypeDef type)
    {
        Name = name;
        Index = index;
        Type = type;
    }

    public string Name { get; }

    public int Index { get; }

    public TypeDef Type { get; }

    public bool Equals(ParameterDef other)
    {
        return Name.Equals(other.Name, StringComparison.InvariantCulture)
            && Index == other.Index
            && Type.Equals(other.Type);
    }

    public override bool Equals(object obj)
    {
        return obj is ParameterDef pd && Equals(pd);
    }

    public override int GetHashCode()
    {
        var hashCode = 1280133078;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
        hashCode = hashCode * -1521134295 + Index.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<TypeDef>.Default.GetHashCode(Type);
        return hashCode;
    }

    public override string ToString()
    {
        return $"{Type} {Name}";
    }
}
