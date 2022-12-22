namespace OneI.Logable.Rendering;

using OneI.Textable;
/// <summary>
/// The logger renderer.
/// </summary>

public interface ILoggerRenderer
{
    /// <summary>
    /// Renders the.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="writer">The writer.</param>
    void Render(LoggerContext context, TextWriter writer);
}
/// <summary>
/// The logger renderer.
/// </summary>

public class LoggerRenderer : ILoggerRenderer
{
    private readonly TemplateContext _context;
    private readonly IFormatProvider? _formatProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerRenderer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="formatProvider">The format provider.</param>
    public LoggerRenderer(TemplateContext context, IFormatProvider? formatProvider = null)
    {
        _context = context;
        _formatProvider = formatProvider;
    }

    /// <summary>
    /// Renders the.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="writer">The writer.</param>
    public void Render(LoggerContext context, TextWriter writer)
    {
        TemplateRenderer.Render(_context, writer, new TemplateOptions(context.GetAllProperties(), _formatProvider));
    }
}
