namespace OneI.Logable.Middlewares;

using System;
/// <summary>
/// The map middleware.
/// </summary>

public class MapMiddleware : ILoggerMiddleware
{
    private readonly Func<LoggerContext, bool> _condition;
    private readonly LoggerDelegate _branch;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapMiddleware"/> class.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="branch">The branch.</param>
    public MapMiddleware(Func<LoggerContext, bool> condition, LoggerDelegate branch)
    {
        _condition = condition;
        _branch = branch;
    }

    /// <summary>
    /// Invokes the.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="next">The next.</param>
    /// <returns>A LoggerVoid.</returns>
    public LoggerVoid Invoke(in LoggerContext context, in LoggerDelegate next)
    {
        if(_condition(context))
        {
            return _branch(context);
        }

        return next(context);
    }
}
