namespace OneI.Logable.Middlewares;
/// <summary>
/// The audit middleware.
/// </summary>

public class AuditMiddleware : ILoggerMiddleware
{
    private readonly Action<LoggerContext> _audit;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditMiddleware"/> class.
    /// </summary>
    /// <param name="audit">The audit.</param>
    public AuditMiddleware(Action<LoggerContext> audit)
    {
        _audit = audit;
    }

    /// <summary>
    /// Invokes the.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="next">The next.</param>
    /// <returns>A LoggerVoid.</returns>
    public LoggerVoid Invoke(in LoggerContext context, in LoggerDelegate next)
    {
        _audit(context);

        return next(context);
    }
}

public delegate void AuditDelegate(in LoggerContext context);
