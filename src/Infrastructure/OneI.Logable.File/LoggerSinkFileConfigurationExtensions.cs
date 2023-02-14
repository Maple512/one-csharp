namespace OneI.Logable;

using OneI.Logable.Internal;

public static class LoggerSinkFileConfigurationExtensions
{
    public static ILoggerConfiguration File(this ILoggerSinkConfiguration logger, string path, Action<LogFileOptions>? configure = null)
    {
        var options = new LogFileOptions(path);

        configure?.Invoke(options);

        return logger.Use(new FileSink(options));
    }

    public static ILoggerConfiguration FileWhen(this ILoggerSinkConfiguration logger
                                                , Func<LoggerContext, bool> condition
                                                , string path
                                                , Action<LogFileOptions>? configure = null)
    {
        var options = new LogFileOptions(path);

        configure?.Invoke(options);

        return logger.UseWhen(condition, new FileSink(options));
    }

    public static ILoggerConfiguration RollFile(this ILoggerSinkConfiguration logger, string path, Action<LogRollFileOptions>? configure = null)
    {
        var options = new LogRollFileOptions(path);

        configure?.Invoke(options);

        return logger.Use(new RollFileSink(options));
    }

    public static ILoggerConfiguration RollFileWhen(this ILoggerSinkConfiguration logger, Func<LoggerContext, bool> condition, string path, Action<LogRollFileOptions>? configure = null)
    {
        var options = new LogRollFileOptions(path);

        configure?.Invoke(options);

        return logger.UseWhen(condition, new RollFileSink(options));
    }
}
