namespace OneI.Logable;

public interface ILoggerMiddleware
{
    LoggerVoid Invoke(in LoggerContext context, in LoggerDelegate next);
}

public readonly struct LoggerVoid
{
    public static readonly LoggerVoid Instance = new();
}
