namespace OneI.Logable.Sinks;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public sealed class DisposingAggregateSink : ILoggerSink, IDisposable, IAsyncDisposable
{
    private readonly IEnumerable<ILoggerSink> _sinks;

    public DisposingAggregateSink(IEnumerable<ILoggerSink> sinks)
    {
        _sinks = sinks;
    }

    public void Emit(LoggerContext context)
    {
        List<Exception>? exceptions = null;
        foreach(var sink in _sinks)
        {
            try
            {
                sink.Emit(context);
            }
            catch(Exception ex)
            {
                // SelfLog.WriteLine("Caught exception while emitting to sink {0}: {1}", sink, ex);
                exceptions ??= new();
                exceptions.Add(ex);
            }
        }

        if(exceptions != null)
        {
            throw new AggregateException("Failed to emit a log event.", exceptions);
        }
    }

    public void Dispose()
    {
        foreach(var sink in _sinks)
        {
            if(sink is not IDisposable disposable)
            {
                continue;
            }

            try
            {
                disposable.Dispose();
            }
            catch(Exception)
            {
                // ReportDisposingException(sink, ex);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach(var sink in _sinks)
        {
            if(sink is IAsyncDisposable asyncDisposable)
            {
                try
                {
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                }
                catch(Exception)
                {
                    // ReportDisposingException(sink, ex);
                }
            }
        }
    }

    //static void ReportDisposingException(ILogEventSink sink, Exception ex)
    //{
    //    SelfLog.WriteLine("Caught exception while disposing sink {0}: {1}", sink, ex);
    //}
}
