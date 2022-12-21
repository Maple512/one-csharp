namespace OneI.Logable.Rendering;

using OneI.Textable;

public interface ILoggerRenderer
{
    void Render(LoggerContext context, TextWriter writer);
}

public class LoggerRenderer : ILoggerRenderer
{
    TemplateContext _context;
    IFormatProvider? _formatProvider;

    public LoggerRenderer(TemplateContext context, IFormatProvider? formatProvider = null)
    {
        _context = context;
        _formatProvider = formatProvider;
    }

    public void Render(LoggerContext context, TextWriter writer)
    {
        TemplateRenderer.Render(_context, writer, new TemplateOptions(context.GetAllProperties(), _formatProvider));
    }
}
