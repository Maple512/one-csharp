namespace OneI.Logable.Internal;

internal static class LogLevelMapper
{
    public static LogLevel ToLogLevel(MSLogging.LogLevel level) => level switch
    {
        MSLogging.LogLevel.Debug => LogLevel.Debug,
        MSLogging.LogLevel.Information => LogLevel.Information,
        MSLogging.LogLevel.Warning => LogLevel.Warning,
        MSLogging.LogLevel.Error => LogLevel.Error,
        MSLogging.LogLevel.Critical => LogLevel.Fatal,
        MSLogging.LogLevel.None or _ => LogLevel.Verbose,
    };

    public static MSLogging.LogLevel ToMSLogLevel(LogLevel level) => level switch
    {
        LogLevel.Debug => MSLogging.LogLevel.Debug,
        LogLevel.Information => MSLogging.LogLevel.Information,
        LogLevel.Warning => MSLogging.LogLevel.Warning,
        LogLevel.Error => MSLogging.LogLevel.Error,
        LogLevel.Fatal => MSLogging.LogLevel.Critical,
        LogLevel.Verbose or _ => MSLogging.LogLevel.Trace,
    };
}
