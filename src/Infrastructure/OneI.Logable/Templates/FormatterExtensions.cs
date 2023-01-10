namespace OneI.Textable;

/// <summary>
/// The formatter extensions.
/// </summary>
public static class FormatterExtensions
{
    /// <summary>
    /// Tos the writer.
    /// </summary>
    /// <param name="formatter">The formatter.</param>
    /// <param name="value">The value.</param>
    /// <param name="format">The format.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>A string.</returns>
    public static string ToDisplayString<T>(this IFormatter<T> formatter, T value, string? format = null, IFormatProvider? formatProvider = null)
    {
        var writer = new StringWriter();

        formatter.Format(value, writer, format, formatProvider);

        return writer.ToString();
    }
}
