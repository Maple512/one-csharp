namespace OneI.Logable.Rendering;

public class LoggerRendererSelector : ILoggerRendererSelector
{
    private readonly List<Func<LoggerContext, ILoggerRenderer?>> _selectors;
    private readonly IFormatProvider? _defaultFormatProvider;

    public LoggerRendererSelector(
        List<Func<LoggerContext, ILoggerRenderer?>> selectors,
        IFormatProvider? defualtFormatProvider = null)
    {
        _selectors = selectors;
        _defaultFormatProvider = defualtFormatProvider;
    }

    public virtual ILoggerRenderer GetRenderer(LoggerContext context)
    {
        foreach(var selector in _selectors)
        {
            var renderer = selector(context);

            if(renderer != null)
            {
                return renderer;
            }
        }

        return new LoggerRenderer(_defaultFormatProvider);
    }
}
