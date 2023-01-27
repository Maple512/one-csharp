namespace OneI.Logable.Templatizations;

using Tokenizations;

public interface ITemplatePropertyValue : IFormattable
{
    void Render(TextWriter writer, PropertyTokenType type, string? format, IFormatProvider? formatProvider);

    public string? ToString()
    {
        return ToString(null, null);
    }
}
