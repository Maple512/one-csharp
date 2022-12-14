namespace OneI.Logable;

using OneI.Logable.Templating.Properties;

public interface ILogger
{
    bool IsEnable(LogLevel level);

    void Write(LoggerContext context);
}
