namespace OneI.Logable.Templates;

public readonly struct EnumerableValue : ITemplatePropertyValue
{
    private readonly List<ITemplatePropertyValue> _values;

    public EnumerableValue()
    {
        _values = new List<ITemplatePropertyValue>();
    }

    public IReadOnlyList<ITemplatePropertyValue> Values => _values;

    public void Render(TextWriter writer, PropertyType type, string? format, IFormatProvider? formatProvider)
    {
        writer.Write('[');

        var length = _values.Count;

        for(var i = 0; i < length; i++)
        {
            _values[i].Render(writer, type, null, formatProvider);

            if(i != length - 1)
            {
                writer.Write(", ");
            }
        }

        writer.Write(']');
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var writer = new StringWriter(formatProvider);

        Render(writer, PropertyType.None, format, formatProvider);

        return writer.ToString();
    }

    public void Add<T>(T value)
    {
        _values.Add(new LiteralValue<T>(value));
    }
}