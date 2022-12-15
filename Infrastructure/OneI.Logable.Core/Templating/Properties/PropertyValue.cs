namespace OneI.Logable.Templating.Properties;

using OneI.Logable.Templating.Properties.ValueTypes;

/// <summary>
/// The property value.
/// </summary>
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

    public static PropertyValue CreateLiteral<T>(T type)
    {
        return new LiteralValue<T>(type);
    }

    public static PropertyValue CreateLiteral<T>(T type, IPropertyValueRenderer<T>? renderer)
    {
        return new LiteralValue<T>(type, renderer);
    }
}
