namespace OneI.Logable;

using OneI.Logable.Configurations;

public static class LoggerTraceSourceExtensions
{
    public static ILoggerConfiguration UseTraceSource(
        this ILoggerSinkConfiguration sink,
        string switchName,
        Action<TraceSourceOptions>? configure = null)
    {
        _ = Check.NotNullOrEmpty(switchName);

        var options = new TraceSourceOptions(switchName);
        configure?.Invoke(options);

        return sink.Use(new TraceSourceSink(options));
    }

    public static ILoggerConfiguration UseTraceSourceWhen(
        this ILoggerSinkConfiguration sink,
        Func<LoggerContext, bool> condition,
        string switchName,
        Action<TraceSourceOptions>? configure = null)
    {
        _ = Check.NotNullOrEmpty(switchName);

        var options = new TraceSourceOptions(switchName);
        configure?.Invoke(options);

        return sink.UseWhen(condition, new TraceSourceSink(options));
    }
}
