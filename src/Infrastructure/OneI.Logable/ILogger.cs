namespace OneI.Logable;

using System.ComponentModel;

public interface ILogger
{
    public static readonly ILogger NullLogger = new None();

    bool IsEnable(LogLevel level) => false;

    void Write(in LoggerContext context) => WriteAsync(context).GetAwaiter().GetResult();

    Task WriteAsync(in LoggerContext context, in CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    ILogger ForContext(params ILoggerMiddleware[] middlewares) => NullLogger;

    IDisposable BeginScope(params ILoggerMiddleware[] middlewares)
        => DisposeAction.Nullable;

    IAsyncDisposable BeginScopeAsync(params ILoggerMiddleware[] middlewares)
        => DisposeAction.Nullable;

    struct None : ILogger { }
}
