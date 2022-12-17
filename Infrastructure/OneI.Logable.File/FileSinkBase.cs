namespace OneI.Logable;

using OneI.Logable.Rendering;

public abstract class FileSinkBase : ILoggerSink
{
    protected readonly List<ITextRendererProvider> _providers;

    protected FileSinkBase(List<ITextRendererProvider> providers)
    {
        _providers = providers;
    }

    public abstract void Invoke(in LoggerContext context);

    protected ILoggerRenderer GetTextRenderer(in LoggerContext context)
    {
        foreach(var provider in _providers)
        {
            var renderer = provider.GetTextRenderer(context);

            if(renderer != null)
            {
                return renderer;
            }
        }

        throw new NotSupportedException($"The renderer can not be null.");
    }
}
