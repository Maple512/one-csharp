namespace OneI.Logable.Templating.Properties;

using OneI.Logable.Templating.Properties.ValueTypes;

public abstract class PropertyValue : IFormattable
{
    public abstract void Render(TextWriter writer, string? format = null, IFormatProvider? formatProvider = null);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var writer = new StringWriter(formatProvider);

        Render(writer, format, formatProvider);

        return writer.ToString();
    }

    public override string ToString() => ToString(null, null);

    public static PropertyValue Create<TType>(TType type)
    {
        return new LiteralValue<TType>(type);
    }
}
