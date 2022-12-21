namespace OneI.Logable.Middlewares;

public class AuditMiddleware : ILoggerMiddleware
{
    private readonly Action<LoggerContext> _audit;

    public AuditMiddleware(Action<LoggerContext> audit)
    {
        _audit = audit;
    }

    public LoggerVoid Invoke(in LoggerContext context, in LoggerDelegate next)
    {
        _audit(context);

        return next(context);
    }
}

public delegate void AuditDelegate(in LoggerContext context);
