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
}
