namespace OneI.Logable.Middlewares;
/// <summary>
/// The null middleware.
/// </summary>

public class NullMiddleware : ILoggerMiddleware
{
    public static readonly ILoggerMiddleware Instance = new NullMiddleware();

    public static readonly LoggerDelegate Delegate = static c => LoggerVoid.Instance;

    /// <summary>
    /// Invokes the.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="next">The next.</param>
    /// <returns>A LoggerVoid.</returns>
    public LoggerVoid Invoke(in LoggerContext context, in LoggerDelegate next)
    {
        return next(context);
    }
}
