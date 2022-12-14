namespace OneI.Logable.Endpoints;

using System;

public class ConditionalSink : ILoggerSink
{
    private readonly Func<LoggerContext, bool> _condition;
    private readonly ILoggerSink _endpoint;

    public ConditionalSink(Func<LoggerContext, bool> condition, ILoggerSink endpoint)
    {
        _condition = condition;
        _endpoint = endpoint;
    }

    public void Invoke(in LoggerContext context)
    {
        if(_condition(context))
        {
            _endpoint.Invoke(context);
        }
    }
}
