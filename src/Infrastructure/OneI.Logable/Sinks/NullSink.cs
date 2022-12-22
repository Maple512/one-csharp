namespace OneI.Logable.Sinks;
/// <summary>
/// The null sink.
/// </summary>

public class NullSink : ILoggerSink
{
    public static readonly ILoggerSink Instance = new NullSink();

    /// <summary>
    /// Invokes the.
    /// </summary>
    /// <param name="context">The context.</param>
    public void Invoke(in LoggerContext context) { }
}
