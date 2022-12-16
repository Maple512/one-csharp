namespace OneI.Logable.Middlewares;

public class AggregateMiddleware : ILoggerMiddleware
{
    private readonly ILoggerMiddleware[] _middlewares;

    public AggregateMiddleware(ILoggerMiddleware[] middlewares)
    {
        _middlewares = middlewares;
    }

    public LoggerVoid Invoke(in LoggerContext context, in LoggerDelegate next)
    {
        foreach(var middleware in _middlewares)
        {
            try
            {
                middleware.Invoke(context, next);
            }
            catch(Exception)
            {
                // todo: log?
            }
        }

        return next(context);
    }
}
