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
}
