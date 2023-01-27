namespace OneI.Logable.Templatizations;

using Tokenizations;

public interface ITemplateSelector
{
    IReadOnlyList<ITemplateToken> Select(LoggerMessageContext context);
}
