namespace OneI.Logable.Middlewares;

using OneI.Logable;
/// <summary>
/// The conditional middleware.
/// </summary>

public class ConditionalMiddleware : ILoggerMiddleware
{
    private readonly Func<LoggerContext, bool> _condition;
    private readonly Func<LoggerContext, LoggerDelegate, LoggerVoid> _middleware;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConditionalMiddleware"/> class.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="middleware">The middleware.</param>
    public ConditionalMiddleware(Func<LoggerContext, bool> condition, ILoggerMiddleware middleware)
    {
        _condition = condition;
        _middleware = (context, next) => middleware.Invoke(context, next);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConditionalMiddleware"/> class.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="middleware">The middleware.</param>
    public ConditionalMiddleware(Func<LoggerContext, bool> condition, Func<LoggerContext, LoggerDelegate, LoggerVoid> middleware)
    {
        _condition = condition;
        _middleware = middleware;
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
            return _middleware.Invoke(context, next);
        }

        return next.Invoke(context);
    }
}
