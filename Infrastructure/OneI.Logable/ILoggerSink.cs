namespace OneI.Logable;

public interface ILoggerSink
{
    void Emit(LoggerContext context);
}
