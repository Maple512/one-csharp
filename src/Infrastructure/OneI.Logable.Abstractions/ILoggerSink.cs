namespace OneI.Logable;

public interface ILoggerSink
{
    void Invoke(in LoggerContext context);
}
