namespace OneI.Logable.Diagnostics;

using OneI.Logable.Configurations;

public static class LoggerDebugConfigurationExtensions
{
    public static ILoggerConfiguration UseDebug(this ILoggerSinkConfiguration logger)
    {
        return logger.Use<DebugSink>();
    }

    public static ILoggerConfiguration UseDebugWhen(
        this ILoggerSinkConfiguration logger,
        Func<LoggerContext, bool> condition)
    {
        return logger.UseWhen<DebugSink>(condition);
    }
}
