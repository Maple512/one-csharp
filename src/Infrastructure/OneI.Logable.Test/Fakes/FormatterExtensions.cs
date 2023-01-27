namespace OneI.Logable.Fakes;

using Templatizations;

public static class FormatterExtensions
{
    public static string ToDisplayString<T>(
        this IPropertyValueFormatter<T> formatter,
        T value,
        string? format = null,
        IFormatProvider? formatProvider = null)
    {
        var writer = new StringWriter();

        formatter.Format(value, writer, format, formatProvider);

        return writer.ToString();
    }
}
