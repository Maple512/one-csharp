namespace OneI.Textable.Templating.Properties;

using OneI.Textable;

public abstract class PropertyValue : IFormattable
{
    public abstract void Render(TextWriter writer, string? format = null, IFormatProvider? formatProvider = null);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var writer = new StringWriter(formatProvider);

        Render(writer, format, formatProvider);

        return writer.ToString();
    }

    public override string ToString()
    {
        return ToString(null, null);
    }

    public static PropertyValue CreateLiteral<T>(T value, IFormatter<T>? formatter = null)
    {
        if(formatter is null && value is IFormatter<T> valueFormatter)
        {
            formatter = valueFormatter;
        }

        return new LiteralValue<T>(value, formatter);
    }
}
