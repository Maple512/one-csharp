namespace OneI.Logable.Rendering;

using OneI.Textable;

public interface ILoggerRenderer
{
    void Render(LoggerContext context, TextWriter writer);
}

public class LoggerRenderer : ILoggerRenderer, IDisposable
{
    private readonly TextTemplate _template;
    private readonly IFormatProvider? _formatProvider;

    public LoggerRenderer(TextTemplate context, IFormatProvider? formatProvider = null)
    {
        _template = context;
        _formatProvider = formatProvider;
    }

    public void Render(LoggerContext context, TextWriter writer)
    {
        _template.WithProperties(context.GetAllProperties());

        _template.Render(writer, _formatProvider);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _template.Dispose();
    }
}
