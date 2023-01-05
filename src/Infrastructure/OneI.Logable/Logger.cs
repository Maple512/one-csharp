namespace OneI.Logable;

using OneI.Logable.Configurations;
using OneI.Logable.Middlewares;

/// <summary>
/// The logger.
/// </summary>
internal class Logger : ILogger
{
    private readonly LogLevelMap _levelMap;
    private AsyncLocal<LoggerDelegate> _middleware = new();
    private readonly ILoggerSink _sink;

    /// <summary>
    /// Initializes a new instance of the <see cref="Logger"/> class.
    /// </summary>
    /// <param name="middleware">The middleware.</param>
    /// <param name="sink">The sink.</param>
    /// <param name="levelMap">The level map.</param>
    internal Logger(
        LoggerDelegate middleware,
        ILoggerSink sink,
        LogLevelMap levelMap)
    {
        _middleware.Value = middleware;
        _sink = sink;
        _levelMap = levelMap;
    }

    /// <summary>
    /// 表示指定的<paramref name="level"/>是否已经启用
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public bool IsEnable(LogLevel level)
    {
        return _levelMap.IsEnabled(level);
    }

    /// <summary>
    /// Writes the.
    /// </summary>
    /// <param name="context">The context.</param>
    public void Write(in LoggerContext context)
    {
        if(IsEnable(context.Level))
        {
            Dispatch(context);
        }
    }

    /// <summary>
    /// Fors the context.
    /// </summary>
    /// <param name="middlewares">The middlewares.</param>
    /// <returns>An ILogger.</returns>
    public ILogger ForContext(params ILoggerMiddleware[] middlewares)
    {
        var aggregate = new AggregateMiddleware(middlewares);

        return new LoggerConfiguration()
            .Use(aggregate)
            .Sink.UseSecondary(this)
            .CreateLogger();
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

        _middleware.Value = @new;

        return new(() => _middleware.Value = old);

        LoggerVoid @new(LoggerContext context)
        {
            return aggregate.Invoke(context, old);
        }
    }

    /// <summary>
    /// Dispatches the.
    /// </summary>
    /// <param name="context">The context.</param>
    private void Dispatch(LoggerContext context)
    {
        try
        {
            _middleware.Value!.Invoke(context);
        }
        catch(Exception)
        {
        }

        _sink.Invoke(context);

        context.Dispose();
    }
}
