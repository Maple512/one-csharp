namespace OneI.Logable;

/// <summary>
/// 表示一个管道中的中间件，用于配置<see cref="LoggerContext"/>
/// </summary>
public interface ILoggerMiddleware
{
    LoggerVoid Invoke(in LoggerContext context, in LoggerDelegate next);
}

public readonly struct LoggerVoid
{
    public static readonly LoggerVoid Instance = new();
}
