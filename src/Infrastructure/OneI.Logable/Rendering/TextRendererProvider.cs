namespace OneI.Logable.Rendering;

public class TextRendererProvider : ITextRendererProvider
{
    private readonly List<Func<LoggerContext, ILoggerRenderer?>> _selector;

    public TextRendererProvider(List<Func<LoggerContext, ILoggerRenderer?>> selector)
    {
        _selector = selector;
    }

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
