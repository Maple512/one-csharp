namespace OneI.Logable;

internal abstract class FileSinkBase : ILoggerSink
{
    protected readonly ITextRendererProvider _provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSinkBase"/> class.
    /// </summary>
    /// <param name="provider">The provider.</param>
    protected FileSinkBase(ITextRendererProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    /// Invokes the.
    /// </summary>
    /// <param name="context">The context.</param>
    public abstract void Invoke(in LoggerContext context);

    /// <summary>
    /// Gets the text renderer.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="writer"></param>
    /// <returns>An ILoggerRenderer.</returns>
    protected virtual void Render(in LoggerContext context, in TextWriter writer)
    {
        var renderer = _provider.GetTextRenderer(context)
            ?? throw new NotSupportedException($"The renderer can not be null.");

        renderer.Render(context, writer);

        if(renderer is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
