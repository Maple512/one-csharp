namespace OneI.Textable.Templating.Properties;
/// <summary>
/// The object value.
/// </summary>

public class ObjectValue : PropertyValue
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectValue"/> class.
    /// </summary>
    public ObjectValue()
    {
        Properties = new();
    }

    /// <summary>
    /// Gets the properties.
    /// </summary>
    public PropertyCollection Properties { get; }

    /// <summary>
    /// Renders the.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="format">The format.</param>
    /// <param name="formatProvider">The format provider.</param>
    public override void Render(TextWriter writer, string? format = null, IFormatProvider? formatProvider = null)
    {
        writer.Write("{ ");
        var index = 0;
        foreach(var item in Properties.Keys)
        {
            writer.Write($"{item}: ");

            Properties[item].Render(writer, format, formatProvider);

            if(index++ != Properties.Count - 1)
            {
                writer.Write(", ");
            }
        }

        writer.Write(" }");
    }

    /// <summary>
    /// Adds the property.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="property">The property.</param>
    public void AddProperty(string name, PropertyValue property)
    {
        Properties.Add(name, property);
    }
}
