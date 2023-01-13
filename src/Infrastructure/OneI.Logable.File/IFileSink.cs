namespace OneI.Logable;

internal interface IFileSink : ILoggerSink, IFileFlusher
{
    bool Write(in LoggerContext context);
}
