namespace OneI.Logable;

public interface IFileSink : ILoggerSink, IFileFlusher
{
    bool Write(in LoggerContext context);
}
