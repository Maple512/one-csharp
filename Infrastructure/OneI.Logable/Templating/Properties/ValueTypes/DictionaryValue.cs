namespace OneI.Logable.Templating.Properties.ValueTypes;

public class DictionaryValue : PropertyValue
{
    private readonly Dictionary<string, PropertyValue> _properties;

    public DictionaryValue()
    {
        _properties = new();
    }

    public IReadOnlyDictionary<string, PropertyValue> Values => _properties;

    public override void Render(TextWriter writer, string? format = null, IFormatProvider? formatProvider = null)
    {
        writer.Write('[');

        var index = 0;
        foreach(var item in Values)
        {
            writer.Write('{');
            new LiteralValue<string>(item.Key).Render(writer, null, formatProvider);

            writer.Write(": ");

            item.Value.Render(writer, null, formatProvider);

            writer.Write('}');
            if(index++ != (Values.Count - 1))
            {
                writer.Write(", ");
            }
        }

        writer.Write(']');
    }

    public void Add(string key, PropertyValue value)
    {
        _properties.TryAdd(key, value);
    }
}
