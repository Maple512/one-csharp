namespace OneI.Logable.Templates;

public readonly struct DictionaryValue : ITemplatePropertyValue
{
    private readonly Dictionary<ITemplatePropertyValue, ITemplatePropertyValue> _properties;

    public DictionaryValue() => _properties = new Dictionary<ITemplatePropertyValue, ITemplatePropertyValue>();

    public IReadOnlyDictionary<ITemplatePropertyValue, ITemplatePropertyValue> Values => _properties;

    public void Render(TextWriter writer, PropertyType type, string? format, IFormatProvider? formatProvider)
    {
        writer.Write('[');

        var index = 0;
        foreach(var item in Values)
        {
            writer.Write('{');

            item.Key.Render(writer, type, null, formatProvider);

            writer.Write(": ");

            item.Value.Render(writer, type, null, formatProvider);

            writer.Write('}');
            if(index++ != Values.Count - 1)
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

    public void Add<T>(string name, T value)
    {
        var key = new LiteralValue<string>(name);

        if(_properties.ContainsKey(key))
        {
            return;
        }

        _properties.Add(key, new LiteralValue<T>(value));
    }

    public void Add(ITemplatePropertyValue key, ITemplatePropertyValue value) => _properties.Add(key, value);
}
