namespace OneI.Logable.Templatizations;

using Tokenizations;

public readonly struct DictionaryValue : ITemplatePropertyValue
{
    private readonly Dictionary<ITemplatePropertyValue, ITemplatePropertyValue> _properties;

    public DictionaryValue()
    {
        _properties = new();
    }

    public IReadOnlyDictionary<ITemplatePropertyValue, ITemplatePropertyValue> Values => _properties;

    public void Render(TextWriter writer, PropertyTokenType type, string? format, IFormatProvider? formatProvider)
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

        Render(writer, PropertyTokenType.None, format, formatProvider);

        return writer.ToString();
    }

    public void Add<T>(string name, T value, IPropertyValueFormatter<T>? formatter = null)
    {
        var key = PropertyValue.CreateLiteral(name);

        if(_properties.ContainsKey(key))
        {
            return;
        }

        _properties.Add(key, PropertyValue.CreateLiteral(value, formatter));
    }

    public void Add(ITemplatePropertyValue key, ITemplatePropertyValue value)
    {
        _properties.Add(key, value);
    }
}
