namespace OneI.Textable.Templating;

using OneI.Textable.Templating.Properties;

public class PropertyCollection : Dictionary<string, PropertyValue>
{
    public PropertyCollection() : base() { }

    public PropertyCollection(Dictionary<string, PropertyValue> properties)
        : base(properties)
    {
    }
}
