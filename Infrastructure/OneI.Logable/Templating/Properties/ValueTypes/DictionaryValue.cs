namespace OneI.Logable.Templating.Properties.ValueTypes;
/// <summary>
/// The dictionary value.
/// </summary>
public class DictionaryValue : PropertyValue
{
    private readonly Dictionary<PropertyValue, PropertyValue> _properties;

    public DictionaryValue()
    {
        _properties = new();
    }

    public IReadOnlyDictionary<PropertyValue, PropertyValue> Values
    {
        get
        {
            return _properties;
        }
    }

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

    public void Add(string name, PropertyValue value)
    {
        var key = Create(name);
        if(_properties.ContainsKey(key))
        {
            return;
        }

        _properties.Add(key, value);
    }

    public void Add(PropertyValue key, PropertyValue value)
    {
        if(_properties.ContainsKey(key))
        {
            return;
        }

        _properties.Add(key, value);
    }
}
