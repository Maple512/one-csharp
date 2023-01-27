namespace OneI.Logable;

using OneI.Logable.Templatizations.Tokenizations;

public class LoggerContext
{
    internal LoggerContext(
        LoggerMessageContext messageContext,
        IReadOnlyList<ITemplateToken> tokens)
    {
        MessageContext = messageContext;
        Tokens = tokens;
    }

    public LoggerMessageContext MessageContext
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    public IReadOnlyList<ITemplateToken> Tokens
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }
}
