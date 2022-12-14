namespace OneI.Logable.Middlewares;

public abstract class LoggerMiddleware : ILoggerMiddleware
{
    public virtual LoggerVoid Invoke(in LoggerContext context, in LoggerDelegate next)
    {
        return LoggerVoid.Instance;
    }
}
