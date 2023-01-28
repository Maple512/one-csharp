namespace OneI.Logable;

using OneI.Logable.Templatizations.Tokenizations;

public readonly struct LoggerContext
{
    internal LoggerContext(
        LoggerMessageContext messageContext,
        IEnumerable<ITemplateToken> tokens)
    {
        MessageContext = messageContext;
        Tokens = tokens;
    }

    public LoggerMessageContext MessageContext
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    public IEnumerable<ITemplateToken> Tokens
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }
}
