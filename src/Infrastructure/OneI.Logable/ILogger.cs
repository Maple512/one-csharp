namespace OneI.Logable;

public interface ILogger
{
    public static readonly ILogger NullLogger = new None();

    bool IsEnable(LogLevel level) => false;

    void Write(in LoggerMessageContext context) { }

    IDisposable BeginScope(params ILoggerMiddleware[] middlewares)
        => DisposeAction.Nullable;

    IAsyncDisposable BeginScopeAsync(params ILoggerMiddleware[] middlewares)
        => DisposeAction.Nullable;

    struct None : ILogger { }
}
