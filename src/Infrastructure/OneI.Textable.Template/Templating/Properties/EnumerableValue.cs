namespace OneI.Textable.Templating.Properties;
/// <summary>
/// The enumerable value.
/// </summary>

public class EnumerableValue : PropertyValue
{
    private readonly List<PropertyValue> _values;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumerableValue"/> class.
    /// </summary>
    public EnumerableValue()
    {
        _values = new List<PropertyValue>();
    }

    /// <summary>
    /// Gets the values.
    /// </summary>
    public IReadOnlyList<PropertyValue> Values => _values;

    /// <summary>
    /// Renders the.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="format">The format.</param>
    /// <param name="formatProvider">The format provider.</param>
    public override void Render(TextWriter writer, string? format = null, IFormatProvider? formatProvider = null)
    {
        writer.Write('[');

        var length = _values.Count;

        for(var i = 0; i < length; i++)
        {
            _values[i].Render(writer, format, formatProvider);

            if(i != length - 1)
            {
                writer.Write(", ");
            }
        }

        writer.Write(']');
    }

    /// <summary>
    /// Adds the property value.
    /// </summary>
    /// <param name="value">The value.</param>
    public void AddPropertyValue(PropertyValue value)
    {
        _values.Add(value);
    }
}
