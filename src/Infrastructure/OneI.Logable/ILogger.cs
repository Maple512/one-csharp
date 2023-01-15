namespace OneI.Logable;

public interface ILogger
{
    public static readonly ILogger NullLogger = new None();

    bool IsEnable(LogLevel level) => false;

    void Write(in LoggerMessageContext context) { }

    ILogger ForContext(ILoggerMiddleware middleware)
    {
        var configuration = new LoggerConfiguration()
            .Use(middleware)
            .Sink.Logger(this);

        return configuration.CreateLogger();
    }

    IDisposable BeginScope(params ILoggerMiddleware[] middlewares)
        => DisposeAction.Nullable;

    IAsyncDisposable BeginScopeAsync(params ILoggerMiddleware[] middlewares)
        => DisposeAction.Nullable;

    struct None : ILogger { }
}
