namespace OneI.Logable.Sinks;

using System;
using System.Collections.Generic;

public class AggregateSink : ILoggerSink
{
    private readonly IReadOnlyList<ILoggerSink> _sinks;

    public AggregateSink(IReadOnlyList<ILoggerSink> sinks)
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
                (exceptions ?? new List<Exception>()).Add(ex);
            }
        }

        if(exceptions != null)
        {
            throw new AggregateException(exceptions);
        }
    }
}
