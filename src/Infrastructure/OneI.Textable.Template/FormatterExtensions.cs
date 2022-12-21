namespace OneI.Textable;

public static class FormatterExtensions
{
    public static string ToWriter<T>(this IFormatter<T> formatter, T value, string? format = null, IFormatProvider? formatProvider = null)
    {
        var writer = new StringWriter();

        formatter.Format(value, writer, format, formatProvider);

        return writer.ToString();
    }
}
