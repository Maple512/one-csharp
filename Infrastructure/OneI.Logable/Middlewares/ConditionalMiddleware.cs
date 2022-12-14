namespace OneI.Logable.Middlewares;

using System;
using OneI.Logable;

public class ConditionalMiddleware : ILoggerMiddleware
{
    private readonly Func<LoggerContext, bool> _condition;
    private readonly Action<LoggerContext> _action;

    public ConditionalMiddleware(Func<LoggerContext, bool> condition, Action<LoggerContext> action)
    {
        _condition = condition;
        _action = action;
    }

    public LoggerVoid Invoke(in LoggerContext context, in LoggerDelegate next)
    {
        if(_condition(context))
        {
            _action(context);
        }

        return next.Invoke(context);
    }
}
