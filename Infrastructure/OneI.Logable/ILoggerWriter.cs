namespace OneI.Logable;

public interface ILoggerWriter
{
    void Write(in LoggerContext context);
}
