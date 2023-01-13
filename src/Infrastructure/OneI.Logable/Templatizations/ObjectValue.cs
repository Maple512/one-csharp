namespace OneI.Logable.Templatizations;

using OneI.Logable.Templatizations.Tokenizations;

public class ObjectValue : PropertyValue
{
    private readonly Dictionary<string, ITemplatePropertyValue> _properties;

    public ObjectValue(int capaticy)
    {
        _properties = new(capaticy);
    }

    public IReadOnlyDictionary<string, ITemplatePropertyValue> Properties => _properties;

    public override void Render(TextWriter writer, in PropertyTokenType type, in string? format = null, IFormatProvider? formatProvider = null)
    {
        writer.Write("{ ");
        var index = 0;
        foreach(var item in _properties.Keys)
        {
            writer.Write($"{item}: ");

            _properties[item].Render(writer, type, format, formatProvider);

            if(index++ != Properties.Count - 1)
            {
                writer.Write(", ");
            }
        }

        writer.Write(" }");
    }

    public void Add<T>(string name, T value, IPropertyValueFormatter<T>? formatter = null)
    {
        _properties.Add(name, CreateLiteral(value, formatter));
    }
}
