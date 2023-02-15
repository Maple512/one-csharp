namespace OneI.Logable;

using System;
using System.Threading.Tasks;
using OneI.Logable.Templates;

[StackTraceHidden]
public class Logger<TSource> : ILogger<TSource>, ILogger
{
    private readonly ILogger _logger;

    public Logger(ILogger logger)
    {
        _logger = logger.ForContext<TSource>();
    }

    IDisposable ILogger.BeginScope(params ILoggerMiddleware[] middlewares)
    {
        return _logger.BeginScope(middlewares);
    }

    IDisposable ILogger.BeginScope<T>(string name, T value)
    {
        return _logger.BeginScope<T>(name, value);
    }

    IAsyncDisposable ILogger.BeginScopeAsync(params ILoggerMiddleware[] middlewares)
    {
        return _logger.BeginScopeAsync(middlewares);
    }

    IAsyncDisposable ILogger.BeginScopeAsync<T>(string name, T value)
    {
        return _logger.BeginScopeAsync(name, value);
    }

    void IDisposable.Dispose()
    {
        _logger.Dispose();
    }

    ValueTask IAsyncDisposable.DisposeAsync()
    {
        return _logger.DisposeAsync();
    }

    ILogger ILogger.ForContext(Action<ILoggerConfiguration> configure)
    {
        return _logger.ForContext(configure);
    }

    ILogger ILogger.ForContext<TValue>(string name, TValue value)
    {
        return _logger.ForContext<TValue>(name, value);
    }

    ILogger ILogger.ForContext(params ILoggerMiddleware[] middlewares)
    {
        return _logger.ForContext(middlewares);
    }

    ILogger ILogger.ForContext<TSourceContext>()
    {
        return _logger.ForContext<TSourceContext>();
    }

    ILogger ILogger.ForContext(string sourceContext)
    {
        return _logger.ForContext(sourceContext);
    }

    bool ILogger.IsEnable(LogLevel level)
    {
        return _logger.IsEnable(level);
    }

    void ILogger.Write(ref LoggerMessageContext context, ref PropertyDictionary properties)
    {
        _logger.Write(ref context, ref properties);
    }
}
