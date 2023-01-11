namespace OneI.Logable.Templating.Properties;

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

    public void Add<T>(T value, IFormatter<T>? formatter = null)
    {
        _values.Add(CreateLiteral(value, formatter));
    }
}
