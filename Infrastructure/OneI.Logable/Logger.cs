namespace OneI.Logable;

using OneI.Logable.Configurations;
using OneI.Logable.Middlewares;
using OneI.Logable.Templating.Properties;

internal class Logger : ILogger
{
    private readonly LogLevelMap _levelMap;
    private readonly LoggerDelegate _middleware;
    private readonly ILoggerSink _sink;

    internal Logger(
        LoggerDelegate middleware,
        ILoggerSink sink,
        LogLevelMap levelMap)
    {
        _middleware = middleware;
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

    public void Write(LoggerContext context)
    {
        if(context is not null
            && IsEnable(context.Level))
        {
            Dispatch(context);
        }
    }

    public ILogger ForContext(params ILoggerMiddleware[] middlewares)
    {
        var aggregate = new AggregateMiddleware(middlewares);

        return new LoggerConfiguration()
            .Use(aggregate)
            .Sink.UseSecondary(this)
            .CreateLogger();
    }

    public ILogger ForContext(string sourceContext)
    {
        return ForContext(new PropertyMiddleware(LoggerConstants.PropertyNames.SourceContext, PropertyValue.CreateLiteral(sourceContext)));
    }

    public ILogger ForContext<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TSourceContext>()
    {
        return ForContext(typeof(TSourceContext).FullName!);
    }

    private void Dispatch(LoggerContext context)
    {
        try
        {
            _middleware.Invoke(context);
        }
        catch(Exception)
        {
        }

        _sink.Invoke(context);
    }
}
