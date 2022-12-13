namespace OneI.Logable.Templating.Formatting;

using Properties;

public static class PropertiesFormatter
{
    public static void Format(
        TextTemplate messageTemplate,
        IReadOnlyDictionary<string, PropertyValue> properties,
        TextTemplate outputTemplate,
        TextWriter writer,
        string? format = null,
        IFormatProvider? formatProvider = null)
    {
        // exclude the same property in the messageTemplate
        var filtered = properties
            .Where(x =>
                    IncludePropertyName(messageTemplate, x.Key) == false
                    || IncludePropertyName(outputTemplate, x.Key) == false);

        writer.Write("{ ");

        var index = 0;
        foreach(var property in filtered)
        {
            writer.Write(property.Key);
            writer.Write(": ");
            property.Value.Render(writer, null, formatProvider);

            if(index++ != properties.Count - 1)
            {
                writer.Write(", ");
            }
        }

        writer.Write(" }");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IncludePropertyName(TextTemplate template, string propertyName) => template.Tokens.OfType<PropertyToken>().Any(x => x.Name == propertyName);
}
