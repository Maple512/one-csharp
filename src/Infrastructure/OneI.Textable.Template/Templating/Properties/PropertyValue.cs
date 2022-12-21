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

    public static PropertyValue CreateLiteral<T>(T value)
    {
        return new LiteralValue<T>(value);
    }

    public static PropertyValue CreateLiteral<T>(T value, IFormatter<T>? renderer)
    {
        return new LiteralValue<T>(value, renderer);
    }
}
