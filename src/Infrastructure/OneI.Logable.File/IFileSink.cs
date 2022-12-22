namespace OneI.Logable;
/// <summary>
/// The file sink.
/// </summary>

internal interface IFileSink : ILoggerSink, IFileFlusher
{
    /// <summary>
    /// Writes the.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>A bool.</returns>
    bool Write(in LoggerContext context);
}
