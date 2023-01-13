namespace OneI.Logable.Templatizations;

using OneI.Logable.Templatizations.Tokenizations;

public interface ITemplateSelector
{
    List<ITemplateToken> Select(in LoggerMessageContext context);
}
