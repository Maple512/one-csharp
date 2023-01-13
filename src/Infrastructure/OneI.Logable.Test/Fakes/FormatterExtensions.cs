namespace OneI.Logable.Fakes;

using OneI.Logable.Templatizations;

public static class FormatterExtensions
{
    public static string ToDisplayString<T>(
        this IPropertyValueFormatter<T> formatter,
        in T value,
        in string? format = null,
        IFormatProvider? formatProvider = null)
    {
        var writer = new StringWriter();

        formatter.Format(value, writer, format, formatProvider);

        return writer.ToString();
    }
}
