namespace OneI.Textable.Templating.Properties;

using OneI.Textable;
/// <summary>
/// The property value.
/// </summary>

public abstract class PropertyValue : IFormattable
{
    /// <summary>
    /// Renders the.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="format">The format.</param>
    /// <param name="formatProvider">The format provider.</param>
    public abstract void Render(TextWriter writer, string? format = null, IFormatProvider? formatProvider = null);

    /// <summary>
    /// Tos the string.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>A string.</returns>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var writer = new StringWriter(formatProvider);

        Render(writer, format, formatProvider);

        return writer.ToString();
    }

    /// <summary>
    /// Tos the string.
    /// </summary>
    /// <returns>A string.</returns>
    public override string ToString()
    {
        return ToString(null, null);
    }

    /// <summary>
    /// Creates the literal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A PropertyValue.</returns>
    public static PropertyValue CreateLiteral<T>(T value)
    {
        return new LiteralValue<T>(value);
    }

    /// <summary>
    /// Creates the literal.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="renderer">The renderer.</param>
    /// <returns>A PropertyValue.</returns>
    public static PropertyValue CreateLiteral<T>(T value, IFormatter<T>? renderer)
    {
        return new LiteralValue<T>(value, renderer);
    }
}
