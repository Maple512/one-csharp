namespace OneI.Logable;

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
