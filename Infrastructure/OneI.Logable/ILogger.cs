namespace OneI.Logable;

public interface ILogger
{
    bool IsEnable(LogLevel level);

    void Write(LoggerContext context);
}
