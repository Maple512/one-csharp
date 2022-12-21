namespace OneI.Logable.Infrastructure;

public class NullLogger : ILogger
{
    public static ILogger Instance => new NullLogger();

    public bool IsEnable(LogLevel level)
    {
        return false;
    }

    public static ILogger New()
    {
        return Instance;
    }

    public void Write(LoggerContext context)
    {
    }

    public ILogger ForContext(params ILoggerMiddleware[] middlewares)
    {
        return Instance;
    }

    public ILogger ForContext(string sourceContext)
    {
        return Instance;
    }

    public ILogger ForContext<TSourceContext>()
    {
        return Instance;
    }
}
