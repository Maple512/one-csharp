namespace OneI.Logable.Endpoints;

using System;

public class ConditionalEndpoint : ILoggerEndpoint
{
    private readonly Func<LoggerContext, bool> _condition;
    private readonly ILoggerEndpoint _endpoint;

    public ConditionalEndpoint(Func<LoggerContext, bool> condition, ILoggerEndpoint endpoint)
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
