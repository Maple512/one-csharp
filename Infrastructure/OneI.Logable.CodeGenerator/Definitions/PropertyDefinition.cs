namespace OneI.Logable.Definitions;

public class PropertyDefinition
{
    public PropertyDefinition(string name, string type)
        : this(name, -1, type)
    {
    }

    public PropertyDefinition(string name, int index, string type)
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
        return Name;
    }
}
