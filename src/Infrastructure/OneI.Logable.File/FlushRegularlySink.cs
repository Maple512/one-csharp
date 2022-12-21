namespace OneI.Logable;

/// <summary>
/// 定期清理缓冲区
/// </summary>
internal class FlushRegularlySink : ILoggerSink, IDisposable
{
    private readonly ILoggerSink _originalSink;
    private readonly Timer _timer;
    private int _flushed;

    public FlushRegularlySink(ILoggerSink originalSink, TimeSpan period)
    {
        _originalSink = Check.NotNull(originalSink);

        if(originalSink is not IFileFlusher flusher)
        {
            throw new NotSupportedException($"The {originalSink.GetType()} is not of {typeof(IFileFlusher)} type");
        }

        _timer = new Timer(_ => FlushToDisk(flusher), null, period, period);
    }

    public void Invoke(in LoggerContext context)
    {
        _originalSink.Invoke(context);

        Interlocked.Exchange(ref _flushed, 0);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _timer.Dispose();

        (_originalSink as IDisposable)?.Dispose();
    }

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
