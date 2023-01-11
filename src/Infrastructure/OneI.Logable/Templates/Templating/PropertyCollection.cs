namespace OneI.Logable.Templating;

using OneI.Logable.Templating.Properties;

public class PropertyCollection : Dictionary<string, PropertyValue>
{
    public PropertyCollection() : base() { }

    public PropertyCollection(Dictionary<string, PropertyValue> properties)
        : base(properties)
    {
    }
}
