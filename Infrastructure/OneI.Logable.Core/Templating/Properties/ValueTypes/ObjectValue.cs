namespace OneI.Logable.Templating.Properties.ValueTypes;

/// <summary>
/// The object value.
/// </summary>
public class ObjectValue : PropertyValue
{
    private readonly List<Property> _values;

    public ObjectValue()
    {
        _values = new();
    }

    public IReadOnlyList<Property> Properties => _values;

    public override void Render(TextWriter writer, string? format = null, IFormatProvider? formatProvider = null)
    {
        writer.Write("{ ");
        var count = _values.Count;
        for(var i = 0; i < count; i++)
        {
            var property = _values[i];

            Render(writer, property, format, formatProvider);

            if(i < count - 1)
            {
                writer.Write(", ");
            }
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

    public void AddProperty(string name, PropertyValue property)
    {
        _values.Add(new Property(name, property));
    }

    public void AddProperty(Property property)
    {
        _values.Add(property);
    }
}
