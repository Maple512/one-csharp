namespace OneI.Logable.Middlewares;

public class NullMiddleware : ILoggerMiddleware
{
    public static readonly ILoggerMiddleware Instance = new NullMiddleware();

    public static readonly LoggerDelegate Delegate = static c => LoggerVoid.Instance;

    public LoggerVoid Invoke(in LoggerContext context, in LoggerDelegate next)
    {
        return next(context);
    }
}
