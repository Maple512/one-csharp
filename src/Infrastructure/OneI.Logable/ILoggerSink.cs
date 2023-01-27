namespace OneI.Logable;

public interface ILoggerSink
{
    void Invoke(LoggerContext context);
}
