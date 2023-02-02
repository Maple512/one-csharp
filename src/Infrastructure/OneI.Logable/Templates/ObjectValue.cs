namespace OneI.Logable.Templates;

public readonly struct ObjectValue : ITemplatePropertyValue
{
    private readonly Dictionary<string, ITemplatePropertyValue> _properties;

    public ObjectValue(int capaticy) => _properties = new Dictionary<string, ITemplatePropertyValue>(capaticy);

    public IReadOnlyDictionary<string, ITemplatePropertyValue> Properties => _properties;

    public void Render(TextWriter writer, PropertyType type, string? format, IFormatProvider? formatProvider)
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

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var writer = new StringWriter(formatProvider);

        Render(writer, PropertyType.None, format, formatProvider);

        return writer.ToString();
    }

    public void Add<T>(string name, T value) => _properties.Add(name, new LiteralValue<T>(value));
}
