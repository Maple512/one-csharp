namespace OneI.Logable.Templates;

public interface ITemplatePropertyValue : IFormattable
{
    void Render(TextWriter writer, PropertyType type, string? format, IFormatProvider? formatProvider);

    public string? ToString()
    {
        return ToString(null, null);
    }
}
