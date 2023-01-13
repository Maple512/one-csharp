namespace OneI.Logable.Templatizations;

using OneI.Logable.Templatizations.Tokenizations;

public abstract class PropertyValue : ITemplatePropertyValue
{
    public abstract void Render(TextWriter writer, in PropertyTokenType type, in string? format = null, IFormatProvider? formatProvider = null);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var writer = new StringWriter(formatProvider);

        Render(writer, PropertyTokenType.None, format, formatProvider);

        return writer.ToString();
    }

    public override string ToString()
    {
        return ToString(null, null);
    }

    public static PropertyValue CreateLiteral<T>(T value, IPropertyValueFormatter<T>? formatter = null)
    {
        if(formatter is null && value is IPropertyValueFormatter<T> valueFormatter)
        {
            formatter = valueFormatter;
        }

        return new LiteralValue<T>(value, formatter);
    }
}
