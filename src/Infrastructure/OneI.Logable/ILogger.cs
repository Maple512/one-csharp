namespace OneI.Logable;

public interface ILogger
{
    bool IsEnable(LogLevel level);

    void Write(LoggerContext context);

    ILogger ForContext(params ILoggerMiddleware[] middlewares);

    ILogger ForContext(string sourceContext);

    ILogger ForContext<TSourceContext>();
}
