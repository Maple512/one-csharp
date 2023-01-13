namespace OneI.Logable.Templatizations;

using Tokenizations;

public interface ITemplatePropertyValue : IFormattable
{
    void Render(TextWriter writer, in PropertyTokenType type, in string? format = null, IFormatProvider? formatProvider = null);
}
