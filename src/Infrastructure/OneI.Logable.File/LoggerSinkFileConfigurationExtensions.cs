namespace OneI.Logable;

using OneI.Logable.Configurations;
using OneI.Textable;
/// <summary>
/// The logger sink file configuration extensions.
/// </summary>

public static class LoggerSinkFileConfigurationExtensions
{
    /// <summary>
    /// Files the.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="path">The path.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>An ILoggerConfiguration.</returns>
    public static ILoggerConfiguration File(
        this ILoggerSinkConfiguration logger,
        string path, Action<LoggerFileOptions>? configuration = null)
    {
        var options = new LoggerFileOptions(path);

        configuration?.Invoke(options);

        return File(logger, options);
    }

    /// <summary>
    /// Files the.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="options">The options.</param>
    /// <returns>An ILoggerConfiguration.</returns>
    public static ILoggerConfiguration File(this ILoggerSinkConfiguration logger, LoggerFileOptions options)
    {
        ILoggerSink sink = new FileSink(
                 options.Path,
                 options.GetProvider(),
                 options.FileSizeMaxBytes,
                 options.Encoding,
                 options.Buffered);

        if(options.FlushRegularly.HasValue)
        {
            sink = new FlushRegularlySink(sink, options.FlushRegularly.Value);
        }

        return logger.Use(sink);
    }

    /// <summary>
    /// Shareds the file.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="path">The path.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>An ILoggerConfiguration.</returns>
    public static ILoggerConfiguration SharedFile(
        this ILoggerSinkConfiguration logger,
        string path,
        Action<LoggerSharedFileOptions>? configuration = null)
    {
        var options = new LoggerSharedFileOptions(path);

        configuration?.Invoke(options);

        return SharedFile(logger, options);
    }

    /// <summary>
    /// Shareds the file.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="options">The options.</param>
    /// <returns>An ILoggerConfiguration.</returns>
    public static ILoggerConfiguration SharedFile(this ILoggerSinkConfiguration logger, LoggerSharedFileOptions options)
    {
        ILoggerSink sink = new SharedFileSink(
                 options.Path,
                 options.GetProvider(),
                 options.FileSizeMaxBytes,
                 options.Encoding);

        if(options.FlushRegularly.HasValue)
        {
            sink = new FlushRegularlySink(sink, options.FlushRegularly.Value);
        }

        return logger.Use(sink);
    }

    /// <summary>
    /// Rolls the file.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="path">The path.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>An ILoggerConfiguration.</returns>
    public static ILoggerConfiguration RollFile(
        this ILoggerSinkConfiguration logger,
        string path,
        Action<LoggerRollFileOptions>? configuration = null)
    {
        var options = new LoggerRollFileOptions(path);

        configuration?.Invoke(options);

        return RollFile(logger, options);
    }

    /// <summary>
    /// Rolls the file.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="options">The options.</param>
    /// <returns>An ILoggerConfiguration.</returns>
    public static ILoggerConfiguration RollFile(this ILoggerSinkConfiguration logger, LoggerRollFileOptions options)
    {
        ILoggerSink sink = new RollFileSink(
                 TemplateParser.Parse(options.Path),
                 options.Frequency,
                 options.GetProvider(),
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
