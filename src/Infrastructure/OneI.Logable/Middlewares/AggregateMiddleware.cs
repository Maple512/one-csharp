namespace OneI.Logable.Middlewares;
/// <summary>
/// The aggregate middleware.
/// </summary>

public class AggregateMiddleware : ILoggerMiddleware
{
    private readonly ILoggerMiddleware[] _middlewares;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateMiddleware"/> class.
    /// </summary>
    /// <param name="middlewares">The middlewares.</param>
    public AggregateMiddleware(ILoggerMiddleware[] middlewares)
    {
        _middlewares = middlewares;
    }

    /// <summary>
    /// Invokes the.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="next">The next.</param>
    /// <returns>A LoggerVoid.</returns>
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
