namespace OneI.Logable.Templating;

using OneI.Logable.Templating.Properties;

public class Property
{
    public Property(string name, PropertyValue value)
    {
        Name = Check.NotNullOrEmpty(name);
        Value = value;
    }

    public string Name { get; }

    public PropertyValue Value { get; }
}
