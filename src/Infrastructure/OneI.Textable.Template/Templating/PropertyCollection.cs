namespace OneI.Textable.Templating;

using OneI.Textable.Templating.Properties;
/// <summary>
/// The property collection.
/// </summary>

public class PropertyCollection : Dictionary<string, PropertyValue>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyCollection"/> class.
    /// </summary>
    public PropertyCollection() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyCollection"/> class.
    /// </summary>
    /// <param name="properties">The properties.</param>
    public PropertyCollection(Dictionary<string, PropertyValue> properties)
        : base(properties)
    {
    }
}
