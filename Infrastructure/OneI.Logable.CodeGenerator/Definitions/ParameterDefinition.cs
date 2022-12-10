namespace OneI.Logable.Definitions;

using OneI.Logable.Infrastructure;

public class ParameterDefinition
{
    public ParameterDefinition(int index, string type)
        : this(index, $"p{index}", type) { }

    public ParameterDefinition(int index, string name, string type)
    {
        Name = name;
        Index = index;
        Type = type;
    }

    public string Name { get; }

    public int Index { get; }

    public string Type { get; }

    public override string ToString()
    {
        return $"{Type} {Name}";
    }

    public void AppendTo(IndentedStringBuilder builder, TypeDefinition type)
    {

    }
}
