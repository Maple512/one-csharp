namespace OneI.Logable.Sinks;

public class NullSink : ILoggerSink
{
    public static readonly ILoggerSink Instance = new NullSink();

    public void Invoke(in LoggerContext context) { }
}
