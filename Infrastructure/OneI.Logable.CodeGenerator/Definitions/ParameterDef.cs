namespace OneI.Logable.Definitions;

public class ParameterDef
{
    public ParameterDef(int index, TypeDef type)
        : this(index, $"p{index}", type) { }

    public ParameterDef(int index, string name, TypeDef type)
    {
        Name = name;
        Index = index;
        Type = type;
    }

    public string Name { get; }

    public int Index { get; }

    public TypeDef Type { get; }

    public override string ToString() => $"{Type} {Name}";
}
