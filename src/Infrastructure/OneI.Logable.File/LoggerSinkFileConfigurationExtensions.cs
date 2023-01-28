namespace OneI.Logable;

using Configurations;

public static class LoggerSinkFileConfigurationExtensions
{
    public static ILoggerConfiguration File(
        this ILoggerSinkConfiguration logger,
        string path,
        Action<LogFileOptions>? configuration = null)
    {
        var options = new LogFileOptions(path);

        configuration?.Invoke(options);

        return logger.Use(new FileSink(options));
    }

    public static ILoggerConfiguration FileWhen(
        this ILoggerSinkConfiguration logger,
        Func<LoggerContext, bool> condition,
        string path,
        Action<LogFileOptions>? configuration = null)
    {
        var options = new LogFileOptions(path);

        configuration?.Invoke(options);

        return logger.UseWhen(condition, new FileSink(options));
    }
}
