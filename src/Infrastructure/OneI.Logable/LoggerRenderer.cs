namespace OneI.Logable;

using System;
using OneI.Logable.Templates;

public class LoggerRenderer : ILoggerRenderer
{
    private readonly IFormatProvider? _formatProvider;

    public LoggerRenderer(IFormatProvider? formatProvider)
    {
        _formatProvider = formatProvider;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(LoggerContext context, TextWriter writer)
    {
        TemplateRenderHelper.Render(writer, context.Template, context.Message, context.Context, _formatProvider);
    }
}
