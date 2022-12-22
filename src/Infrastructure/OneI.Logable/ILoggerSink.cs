namespace OneI.Logable;
/// <summary>
/// The logger sink.
/// </summary>

public interface ILoggerSink
{
    /// <summary>
    /// Invokes the.
    /// </summary>
    /// <param name="context">The context.</param>
    void Invoke(in LoggerContext context);
}
