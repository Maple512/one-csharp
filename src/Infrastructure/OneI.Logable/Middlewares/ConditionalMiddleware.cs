namespace OneI.Logable.Middlewares;

using OneI.Logable;

public class ConditionalMiddleware : ILoggerMiddleware
{
    private readonly Func<LoggerContext, bool> _condition;
    private readonly Func<LoggerContext, LoggerDelegate, LoggerVoid> _middleware;

    public ConditionalMiddleware(Func<LoggerContext, bool> condition, ILoggerMiddleware middleware)
    {
        _condition = condition;
        _middleware = (context, next) => middleware.Invoke(context, next);
    }

    public ConditionalMiddleware(Func<LoggerContext, bool> condition, Func<LoggerContext, LoggerDelegate, LoggerVoid> middleware)
    {
        _condition = condition;
        _middleware = middleware;
    }

    public LoggerVoid Invoke(in LoggerContext context, in LoggerDelegate next)
    {
        if(_condition(context))
        {
            return _middleware.Invoke(context, next);
        }

        return next.Invoke(context);
    }
}
