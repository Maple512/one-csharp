namespace OneI.Logable.Templating.Properties.ValueTypes;

public class ObjectValue : PropertyValue
{
    const string _type = "$type";

    private readonly List<Property> _values;

    public ObjectValue(string originalType)
    {
        OriginalType = originalType;
        _values = new();
    }

    /// <summary>
    /// 原始的类型
    /// </summary>
    public string OriginalType { get; }

    public IReadOnlyList<Property> Properties => _values;

    public override void Render(TextWriter writer, string? format = null, IFormatProvider? formatProvider = null)
    {
        writer.Write("{ ");
        var count = _values.Count;
        for(var i = 0; i < count; i++)
        {
            var property = _values[i];

            Render(writer, property, format, formatProvider);

            writer.Write(", ");
        }

        if(OriginalType.NotNullOrWhiteSpace())
        {
            Render(writer, new Property(_type, new LiteralValue<string>(OriginalType)), format, formatProvider);
        }

        writer.Write(" }");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Render(TextWriter writer, Property property, string? format = null, IFormatProvider? formatProvider = null)
    {
        new LiteralValue<string>(property.Name).Render(writer);
        writer.Write(": ");
        property.Value.Render(writer, format, formatProvider);
    }

    public void AddProperty(Property property)
    {
        _values.Add(property);
    }
}
