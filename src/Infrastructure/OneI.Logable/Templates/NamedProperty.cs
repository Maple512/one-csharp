namespace OneI.Logable.Templates;

public class NamedProperty : ITemplateProperty
{
    public NamedProperty(string name, ITemplatePropertyValue value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; }

    public ITemplatePropertyValue Value { get; }
}
