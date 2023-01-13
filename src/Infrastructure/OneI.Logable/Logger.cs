namespace OneI.Logable;

using OneI.Logable.Middlewares;
using OneI.Logable.Templatizations;

internal class Logger : ILogger
{
    private readonly LogLevelMap _levelMap;
    private readonly AsyncLocal<ILoggerMiddleware> _middleware;
    private readonly ILoggerSink _sink;
    private readonly ITemplateSelector _templateSelector;

    internal Logger(
        ILoggerMiddleware middleware,
        ILoggerSink sink,
        LogLevelMap levelMap,
        ITemplateSelector templateSelector)
    {
        _middleware = new()
        {
            Value = middleware,
        };

        _sink = sink;
        _levelMap = levelMap;
        _templateSelector = templateSelector;
    }

    public bool IsEnable(LogLevel level)
    {
        return _levelMap.IsEnabled(level);
    }

    public void Write(in LoggerMessageContext context)
    {
        if(IsEnable(context.Level))
        {
            Dispatch(context);
        }
    }

    public IDisposable BeginScope(params ILoggerMiddleware[] middlewares)
    {
        return CreateScope(middlewares);
    }

    public IAsyncDisposable BeginScopeAsync(params ILoggerMiddleware[] middlewares)
    {
        return CreateScope(middlewares);
    }

    private DisposeAction CreateScope(params ILoggerMiddleware[] middlewares)
    {
        var aggregate = new AggregateMiddleware(middlewares);

        var old = _middleware.Value!;

        _middleware.Value = new ActionMiddleware(context =>
        {
            aggregate.Invoke(context);
        });

        return new(() => _middleware.Value = old);
    }

    private void Dispatch(in LoggerMessageContext context)
    {
        try
        {
            _middleware.Value!.Invoke(context);
        }
        catch(Exception)
        {
        }

        var tokens = _templateSelector.Select(context);

        var properties = context.GetProperties();

        var loggerContext = new LoggerContext(tokens, properties.ToList(), context.ToReadOnly());

        _sink.Invoke(loggerContext);
    }
}
