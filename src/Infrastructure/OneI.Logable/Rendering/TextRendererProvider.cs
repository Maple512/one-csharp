namespace OneI.Logable.Rendering;
/// <summary>
/// The text renderer provider.
/// </summary>

public class TextRendererProvider : ITextRendererProvider
{
    private readonly List<Func<LoggerContext, ILoggerRenderer?>> _selector;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextRendererProvider"/> class.
    /// </summary>
    /// <param name="selector">The selector.</param>
    public TextRendererProvider(List<Func<LoggerContext, ILoggerRenderer?>> selector)
    {
        _selector = selector;
    }

    /// <summary>
    /// Gets the text renderer.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>An ILoggerRenderer.</returns>
    public virtual ILoggerRenderer GetTextRenderer(in LoggerContext context)
    {
        foreach(var selector in _selector)
        {
            var renderer = selector(context);

            if(renderer != null)
            {
                return renderer;
            }
        }

        throw new LoggerRendererNotFoundException();
    }
}
