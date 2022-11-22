namespace OneI.Logable;

using System;
using System.Threading;

public class PeriodicFlushToDiskSink : ILoggerSink, IDisposable
{
    private readonly ILoggerSink _sink;
    private readonly Timer _timer;
    private int _flushRequired;

    public PeriodicFlushToDiskSink(ILoggerSink sink, TimeSpan flushInterval)
    {
        _sink = sink ?? throw new ArgumentNullException(nameof(sink));

        if(sink is IFileFlusher fileSink)
        {
            _timer = new Timer(_ => FlushToDisk(fileSink), null, flushInterval, flushInterval);
        }
        else
        {
            _timer = new Timer(_ => { }, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }
    }

    public void Dispose()
    {
        _timer.Dispose();

        (_sink as IDisposable)?.Dispose();

        GC.SuppressFinalize(this);
    }

    public void Emit(LoggerContext context)
    {
        _sink.Emit(context);

        Interlocked.Exchange(ref _flushRequired, 1);
    }

    private void FlushToDisk(IFileFlusher fileFlusher)
    {
        try
        {
            // is require or not
            if(Interlocked.CompareExchange(ref _flushRequired, 0, 1) == 1)
            {
                fileFlusher.Flush();
            }
        }
        catch(Exception ex)
        {
            InternalLog.WriteLine("{o} could not flush the underlaying sink to disk: {1}", typeof(PeriodicFlushToDiskSink), ex);
        }
    }
}
