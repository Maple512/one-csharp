namespace OneI.Logable;

public interface IFileSink : ILoggerSink, IFileFlusher
{
    bool EmitOrOverflow(LoggerContext context);
}
