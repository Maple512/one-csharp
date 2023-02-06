namespace OneI.Logable;

using System.Runtime.Versioning;
using OneI.Logable.Configurations;

[SupportedOSPlatform(SharedConstants.OSPlatform.Windows)]
public static class LoggerEventSourceExtensions
{
    public static ILoggerConfiguration UseEventSource(
        this ILoggerSinkConfiguration sink,
        Action<EventSourceOptions>? configure)
    {
        var options = new EventSourceOptions();
        configure?.Invoke(options);

        options.VerifyAndCreateSource();

        return sink.Use(new EventSourceSink(options));
    }

    public static ILoggerConfiguration UseEventSourceWhen(
        this ILoggerSinkConfiguration sink,
        Func<LoggerContext, bool> condition,
        Action<EventSourceOptions>? configure)
    {
        var options = new EventSourceOptions();
        configure?.Invoke(options);
        options.VerifyAndCreateSource();

        return sink.UseWhen(condition, new EventSourceSink(options));
    }
}