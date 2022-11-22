namespace OneI.Logable;

using System;
using System.Text;
using OneI.Logable.Configurations;
using OneI.Logable.Formatting;
using OneI.Logable.Rolling;
using OneI.Logable.Sinks;

public static class FileConfigurationExtensions
{
    private const int DefaultRetainedFileMaxCount = 31; // A long month of logs
    private const long DefaultFileMaxSizeBytes = 1L * 1024 * 1024 * 1024; // 1GB
    private const string DefaultLoggerMessageTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

    public static LoggerConfiguration File(
        this LoggerSinkConfiguration sinkConfiguration,
        string path,
        string template = DefaultLoggerMessageTemplate,
        LogLevel minimumLevel = LogLevelSwitch.Minimum,
        LogLevelSwitch? levelSwitch = null,
        long? fileMaxSizeBytes = DefaultFileMaxSizeBytes,
        bool buffered = false,
        bool propagateExceptions = true,
        bool shared = false,
        TimeSpan? flushToDiskInternal = null,
        Encoding? encoding = null,
        RollingPolicy policy = RollingPolicy.Motionless,
        bool rollOnFileMaxSize = false,
        int? retainedFileMaxCount = DefaultRetainedFileMaxCount,
        FileLifecycleHooks? hooks = null,
        TimeSpan? retainedFileMaxTime = null,
        IFormatProvider? formatProvider = null)
    {
        var formatter = new TextFormatter(template, formatProvider);

        return File(
            sinkConfiguration.Sink,
            formatter,
            path,
            minimumLevel,
            levelSwitch,
            fileMaxSizeBytes,
            buffered,
            propagateExceptions,
            shared,
            flushToDiskInternal,
            encoding,
             policy,
             rollOnFileMaxSize,
             retainedFileMaxCount,
             hooks,
             retainedFileMaxTime);
    }

    public static LoggerConfiguration File(
        Func<ILoggerSink, LogLevel, LogLevelSwitch?, LoggerConfiguration> appendSinkAction,
        ITextFormatter textFormatter,
        string path,
        LogLevel minimumLevel,
        LogLevelSwitch? levelSwitch,
        long? fileMaxSizeBytes,
        bool buffered,
        bool propagateExceptions,
        bool shared,
        TimeSpan? flushToDiskInternal,
        Encoding? encoding,
        RollingPolicy policy,
        bool rollOnFileMaxSize,
        int? retainedFileMaxCount,
        FileLifecycleHooks? hooks,
        TimeSpan? retainedFileMaxTime)
    {
        CheckTools.NotNull(path);

        ILoggerSink sink;
        try
        {
            if(rollOnFileMaxSize
                || policy != RollingPolicy.Motionless)
            {
                sink = new RollingFileSink(
                    path,
                    textFormatter,
                    fileMaxSizeBytes,
                    retainedFileMaxCount,
                    encoding,
                    buffered,
                    shared,
                    policy,
                    rollOnFileMaxSize,
                    hooks,
                    retainedFileMaxTime);
            }
            else
            {
                if(shared)
                {
                    sink = new SharedFileSink(path, textFormatter, fileMaxSizeBytes, encoding);
                }
                else
                {
                    sink = new FileSink(path, textFormatter, fileMaxSizeBytes, buffered, encoding);
                }
            }
        }
        catch(Exception ex)
        {
            InternalLog.WriteLine("Unable to open file sink for {0}: {1}", path, ex);

            if(propagateExceptions)
            {
                throw;
            }

            return appendSinkAction(new NullableSink(), minimumLevel, levelSwitch);
        }

        if(flushToDiskInternal.HasValue)
        {
            sink = new PeriodicFlushToDiskSink(sink, flushToDiskInternal.Value);
        }

        return appendSinkAction(sink, minimumLevel, levelSwitch);
    }
}
