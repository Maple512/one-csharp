namespace OneI.Logable.Sinks;

using System;

public class ConditionalSink : ILoggerSink
{
    private readonly Func<LoggerContext, bool> _condition;
    private readonly Action<LoggerContext> _sink;

    public ConditionalSink(Func<LoggerContext, bool> condition, Action<LoggerContext> sink)
    {
        _condition = condition;
        _sink = sink;
    }

    public void Invoke(in LoggerContext context)
    {
        if(_condition(context))
        {
            _sink.Invoke(context);
        }
    }
}
