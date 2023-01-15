namespace OneI.Logable.Templatizations;

using OneI.Logable.Templatizations.Tokenizations;

public class EnumerableValue : PropertyValue
{
    private readonly List<ITemplatePropertyValue> _values;

    public EnumerableValue()
    {
        _values = new List<ITemplatePropertyValue>();
    }

    public IReadOnlyList<ITemplatePropertyValue> Values => _values;

    public override void Render(TextWriter writer, in PropertyTokenType type, in string? format = null, IFormatProvider? formatProvider = null)
    {
        writer.Write('[');

        var length = _values.Count;

        for(var i = 0; i < length; i++)
        {
            _values[i].Render(writer, type, format, formatProvider);

            if(i != length - 1)
            {
                writer.Write(", ");
            }
        }

        writer.Write(']');
    }

    public void Add<T>(T value, IPropertyValueFormatter<T>? formatter = null)
    {
        _values.Add(CreateLiteral(value, formatter));
    }
}
