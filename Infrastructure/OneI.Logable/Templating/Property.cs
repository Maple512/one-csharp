namespace OneI.Logable.Templating;

using OneI.Logable.Templating.Properties;

public class Property
{
    public Property(string name, PropertyValue value)
    {
        Name = CheckTools.NotNullOrEmpty(name);
        Value = value;
    }

    public string Name { get; }

    public PropertyValue Value { get; }
}
