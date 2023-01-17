namespace OneI.Logable.Templatizations;

using OneI.Logable.Templatizations.Tokenizations;

public class DictionaryValue : PropertyValue
{
    private readonly Dictionary<ITemplatePropertyValue, ITemplatePropertyValue> _properties;

    public DictionaryValue()
    {
        _properties = new();
    }

    public IReadOnlyDictionary<ITemplatePropertyValue, ITemplatePropertyValue> Values => _properties;

    public override void Render(TextWriter writer, in PropertyTokenType type, in string? format, IFormatProvider? formatProvider)
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

    public void Add<T>(string name, T value, IPropertyValueFormatter<T>? formatter = null)
    {
        var key = CreateLiteral(name);

        if(_properties.ContainsKey(key))
        {
            return;
        }

        _properties.Add(key, CreateLiteral(value, formatter));
    }

    public void Add(ITemplatePropertyValue key, ITemplatePropertyValue value)
    {
        _properties.Add(key, value);
    }
}
