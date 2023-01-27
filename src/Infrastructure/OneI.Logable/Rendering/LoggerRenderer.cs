namespace OneI.Logable.Rendering;

using OneI.Logable.Templatizations;

public class LoggerRenderer : ILoggerRenderer
{
    private readonly IFormatProvider? _formatProvider;

    public LoggerRenderer(IFormatProvider? formatProvider)
    {
        _formatProvider = formatProvider;
    }

    public void Render(LoggerContext context, TextWriter writer)
    {
        TemplateContext.Render(writer, context.Tokens, context.MessageContext, _formatProvider);
    }
}
