namespace OneI.Logable.Middlewares;

using System;

public class MapMiddleware : LoggerMiddleware
{
    private readonly Func<LoggerContext, bool> _condition;
    private readonly LoggerDelegate _branch;

    public MapMiddleware(Func<LoggerContext, bool> condition, LoggerDelegate branch)
    {
        _condition = condition;
        _branch = branch;
    }

    public override LoggerVoid Invoke(in LoggerContext context, in LoggerDelegate next)
    {
        if(_condition(context))
        {
            return _branch(context);
        }

        return next(context);
    }
}
