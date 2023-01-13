namespace OneI.Logable;

using OneI.Logable.Templatizations;
using OneI.Logable.Templatizations.Tokenizations;

public class LoggerContext
{
    internal LoggerContext(
        IReadOnlyList<ITemplateToken> tokens,
        IReadOnlyList<ITemplateProperty> properties,
        ReadOnlyMessageContext messageContext)
    {
        Tokens = tokens;
        Properties = properties;
        MessageContext = messageContext;
    }

    public ReadOnlyMessageContext MessageContext
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    public IReadOnlyList<ITemplateToken> Tokens { get; }

    public IReadOnlyList<ITemplateProperty> Properties { get; }
}
