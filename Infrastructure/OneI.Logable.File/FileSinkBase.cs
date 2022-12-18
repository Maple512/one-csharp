namespace OneI.Logable;

using OneI.Logable.Rendering;

internal abstract class FileSinkBase : ILoggerSink
{
    protected readonly ITextRendererProvider _provider;

    protected FileSinkBase(ITextRendererProvider provider)
    {
        _provider = provider;
    }

    public abstract void Invoke(in LoggerContext context);

    protected ILoggerRenderer GetTextRenderer(in LoggerContext context)
    {
        return _provider.GetTextRenderer(context)
            ?? throw new NotSupportedException($"The renderer can not be null.");
    }
}
