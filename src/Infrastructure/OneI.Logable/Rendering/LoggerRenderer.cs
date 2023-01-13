namespace OneI.Logable.Rendering;

using System.IO;
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
        var template = new TemplateContext(context.Tokens, context.Properties);

        template.Render(writer, _formatProvider);
    }
}
