namespace OneI.Textable.Templating.Properties;
/// <summary>
/// The dictionary value.
/// </summary>

public class DictionaryValue : PropertyValue
{
    private readonly Dictionary<PropertyValue, PropertyValue> _properties;

    /// <summary>
    /// Initializes a new instance of the <see cref="DictionaryValue"/> class.
    /// </summary>
    public DictionaryValue()
    {
        _properties = new();
    }

    /// <summary>
    /// Gets the values.
    /// </summary>
    public IReadOnlyDictionary<PropertyValue, PropertyValue> Values => _properties;

    /// <summary>
    /// Renders the.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="format">The format.</param>
    /// <param name="formatProvider">The format provider.</param>
    public override void Render(TextWriter writer, string? format = null, IFormatProvider? formatProvider = null)
    {
        writer.Write('[');

        var index = 0;
        foreach(var item in Values)
        {
            writer.Write('{');

            item.Key.Render(writer, null, formatProvider);

            writer.Write(": ");

            item.Value.Render(writer, null, formatProvider);

            writer.Write('}');
            if(index++ != Values.Count - 1)
            {
                writer.Write(", ");
            }
        }

        writer.Write(']');
    }

    public void Add<T>(string name, T value, IFormatter<T>? formatter = null)
    {
        var key = CreateLiteral(name);
        if(_properties.ContainsKey(key))
        {
            return;
        }

        _properties.Add(key, CreateLiteral(value, formatter));
    }
}
