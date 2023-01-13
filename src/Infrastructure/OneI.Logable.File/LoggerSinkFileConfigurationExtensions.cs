namespace OneI.Logable;

using OneI.Logable.Configurations;

public static class LoggerSinkFileConfigurationExtensions
{
    public static ILoggerConfiguration File(
        this ILoggerSinkConfiguration logger,
        string path, Action<LoggerFileOptions>? configuration = null)
    {
        var options = new LoggerFileOptions(path);

        configuration?.Invoke(options);

        return File(logger, options);
    }

    public static ILoggerConfiguration File(this ILoggerSinkConfiguration logger, LoggerFileOptions options)
    {
        ILoggerSink sink = new FileSink(
                 options.Path,
                 options.GetLoggerRenderer(),
                 options.FileSizeMaxBytes,
                 options.Encoding,
                 options.Buffered);

        if(options.FlushRegularly.HasValue)
        {
            sink = new FlushRegularlySink(sink, options.FlushRegularly.Value);
        }

        return logger.Use(sink);
    }

    public static ILoggerConfiguration SharedFile(
        this ILoggerSinkConfiguration logger,
        string path,
        Action<LoggerSharedFileOptions>? configuration = null)
    {
        var options = new LoggerSharedFileOptions(path);

        configuration?.Invoke(options);

        return SharedFile(logger, options);
    }

    public static ILoggerConfiguration SharedFile(this ILoggerSinkConfiguration logger, LoggerSharedFileOptions options)
    {
        ILoggerSink sink = new SharedFileSink(
                 options.Path,
                 options.GetLoggerRenderer(),
                 options.FileSizeMaxBytes,
                 options.Encoding);

        if(options.FlushRegularly.HasValue)
        {
            sink = new FlushRegularlySink(sink, options.FlushRegularly.Value);
        }

        return logger.Use(sink);
    }

    public static ILoggerConfiguration RollFile(
        this ILoggerSinkConfiguration logger,
        string path,
        Action<LoggerRollFileOptions>? configuration = null)
    {
        var options = new LoggerRollFileOptions(path);

        configuration?.Invoke(options);

        return RollFile(logger, options);
    }

    public static ILoggerConfiguration RollFile(this ILoggerSinkConfiguration logger, LoggerRollFileOptions options)
    {
        ILoggerSink sink = new RollFileSink(
                 options.Path,
                 options.Frequency,
                 options.GetLoggerRenderer(),
                 options.FileSizeMaxBytes,
                 options.RetainedFileCountMax,
                 options.RetainedFileTimeMax,
                 options.Encoding,
                 options.Buffered,
                 options.IsShared);

        if(options.FlushRegularly.HasValue)
        {
            sink = new FlushRegularlySink(sink, options.FlushRegularly.Value);
        }

        return logger.Use(sink);
    }
}
