namespace OneI.Logable.Sinks;

using System;
using System.Collections.Generic;

public class SafeAggregateSink : ILoggerSink
{
    private readonly IReadOnlyList<ILoggerSink> _sinks;

    public SafeAggregateSink(IReadOnlyList<ILoggerSink> sinks)
    {
        _sinks = sinks;
    }

    public void Emit(LoggerContext context)
    {
        foreach(var sink in _sinks)
        {
            try
            {
                sink.Emit(context);
            }
            catch(Exception) { }
        }
    }
}
