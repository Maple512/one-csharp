namespace OneI.Logable;

/// <summary>
/// 定期清理缓冲区
/// </summary>
internal class FlushRegularlySink : ILoggerSink, IDisposable
{
    private readonly ILoggerSink _originalSink;
    private readonly Timer _timer;
    private int _flushed;

    /// <summary>
    /// Initializes a new instance of the <see cref="FlushRegularlySink"/> class.
    /// </summary>
    /// <param name="originalSink">The original sink.</param>
    /// <param name="period">The period.</param>
    public FlushRegularlySink(ILoggerSink originalSink, TimeSpan period)
    {
        _originalSink = Check.NotNull(originalSink);

        if(originalSink is not IFileFlusher flusher)
        {
            throw new NotSupportedException($"The {originalSink.GetType()} is not of {typeof(IFileFlusher)} type");
        }

        _timer = new Timer(_ => FlushToDisk(flusher), null, period, period);
    }

    /// <summary>
    /// Invokes the.
    /// </summary>
    /// <param name="context">The context.</param>
    public void Invoke(in LoggerContext context)
    {
        _originalSink.Invoke(context);

        Interlocked.Exchange(ref _flushed, 0);
    }

    /// <summary>
    /// Disposes the.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _timer.Dispose();

        (_originalSink as IDisposable)?.Dispose();
    }

    /// <summary>
    /// Flushes the to disk.
    /// </summary>
    /// <param name="flusher">The flusher.</param>
    private void FlushToDisk(IFileFlusher flusher)
    {
        try
        {
            if(Interlocked.CompareExchange(ref _flushed, 1, 0) == 0)
            {
                flusher.FlushToDisk();
            }
        }
        catch(Exception ex)
        {
            InternalLog.WriteLine($"Could not flush the original sink to disk: {ex}");
        }
    }
}
