namespace OneI.Logable;

using OneI.Logable.Templates;

public readonly struct LoggerContext
{
    public LoggerContext(LoggerMessageContext context, TemplateEnumerator template, TemplateEnumerator message)
    {
        Context = context;
        Template = template;
        Message = message;
    }

    public LoggerMessageContext Context
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    public TemplateEnumerator Template
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    public TemplateEnumerator Message
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }
}
