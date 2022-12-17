namespace OneI.Textable.Templating.Properties;

public class EnumerableValue : PropertyValue
{
    private readonly List<PropertyValue> _values;

    public EnumerableValue()
    {
        _values = new List<PropertyValue>();
    }

    public IReadOnlyList<PropertyValue> Values => _values;

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

    public void AddPropertyValue(PropertyValue value)
    {
        _values.Add(value);
    }
}
