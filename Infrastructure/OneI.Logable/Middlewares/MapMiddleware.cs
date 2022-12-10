namespace OneI.Logable.Middlewares;

public class MapMiddleware : ILoggerMiddleware
{
    readonly LoggerDelegate _branch;
    readonly Func<LoggerContext, bool> _condition;

    public MapMiddleware(LoggerDelegate branch, Func<LoggerContext, bool> condition)
    {
        _branch = branch;
        _condition = condition;
    }

    public void Invoke(in LoggerContext context, in LoggerDelegate next)
    {
        if(_condition(context))
        {
            _branch(context);
        }

        next(context);
    }
}
