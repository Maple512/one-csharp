namespace OneI.Logable.Configurations;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using OneI.Logable.Sinks;

public class LoggerSinkConfiguration
{
    private readonly LoggerConfiguration _loggerConfiguration;
    private readonly Action<ILoggerSink> _sinkAction;

    public LoggerSinkConfiguration(LoggerConfiguration loggerConfiguration, Action<ILoggerSink> sinkAction)
    {
        _loggerConfiguration = loggerConfiguration;
        _sinkAction = sinkAction;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public LoggerConfiguration Sink(
        ILoggerSink logEventSink,
        LogLevel restrictedToMinimumLevel)
    {
        return Sink(logEventSink, restrictedToMinimumLevel, null);
    }

    public LoggerConfiguration Sink<TSink>(
        LogLevel restrictedToMinimumLevel = LogLevelSwitch.Minimum,
        LogLevelSwitch? levelSwitch = null)
        where TSink : ILoggerSink, new()
    {
        return Sink(new TSink(), restrictedToMinimumLevel, levelSwitch);
    }

    public LoggerConfiguration Sink(
        ILoggerSink logEventSink,
        LogLevel restrictedToMinimumLevel = LogLevelSwitch.Minimum,
        LogLevelSwitch? levelSwitch = null)
    {
        var sink = logEventSink;
        if(levelSwitch != null)
        {
            if(restrictedToMinimumLevel != LogLevelSwitch.Minimum)
            {
                // SelfLog.WriteLine("Sink {0} was configured with both a level switch and minimum level '{1}'; the minimum level will be ignored and the switch level used", sink, restrictedToMinimumLevel);

                sink = new RestrictedSink(levelSwitch.Value, sink);
            }
        }
        else if(restrictedToMinimumLevel > LogLevelSwitch.Minimum)
        {
            sink = new RestrictedSink(new(restrictedToMinimumLevel), sink);
        }

        _sinkAction(sink);

        return _loggerConfiguration;
    }

    public LoggerConfiguration Logger(
        Action<LoggerConfiguration> configureLogger,
        LogLevel restrictedToMinimumLevel = LogLevelSwitch.Minimum,
        LogLevelSwitch? levelSwitch = null)
    {
        var lc = new LoggerConfiguration().MinimumLevel.Minimum(LogLevelSwitch.Minimum);
        configureLogger(lc);

        var subLogger = (Logger)lc.CreateLogger();

        if(subLogger.HasOverrideMap)
        {
            InternalLog.WriteLine("Minimum level overrides are not supported on sub-loggers and may be removed completely in a future version.");
        }

        var secondarySink = new SecondaryLoggerSink(subLogger, true);

        return Sink(secondarySink, restrictedToMinimumLevel, levelSwitch);
    }

    public LoggerConfiguration Logger(
        ILogger logger,
        LogLevel restrictedToMinimumLevel = LogLevelSwitch.Minimum)
    {
        if(logger is Logger { HasOverrideMap: true })
        {
            InternalLog.WriteLine("Minimum level overrides are not supported on sub-loggers and may be removed completely in a future version.");
        }

        var secondarySink = new SecondaryLoggerSink(logger, attemptDispose: false);

        return Sink(secondarySink, restrictedToMinimumLevel);
    }

    public LoggerConfiguration Conditional(Func<LoggerContext, bool> condition, Action<LoggerSinkConfiguration> configureSink)
    {
        // Level aliases and so on don't need to be accepted here; if the user wants both a condition and leveling, they
        // can specify `restrictedToMinimumLevel` etc in the wrapped sink configuration.
        return Wrap(
            this,
            s => new ConditionalSink(s, condition),
            configureSink,
            LogLevelSwitch.Minimum,
            null);
    }

    public static LoggerConfiguration Wrap(
        LoggerSinkConfiguration loggerSinkConfiguration,
        Func<ILoggerSink, ILoggerSink> wrapSink,
        Action<LoggerSinkConfiguration> configureWrappedSink,
        LogLevel restrictedToMinimumLevel,
        LogLevelSwitch? levelSwitch)
    {
        var sinksToWrap = new List<ILoggerSink>();

        var capturingConfiguration = new LoggerConfiguration();
        var capturingLoggerSinkConfiguration = new LoggerSinkConfiguration(
            capturingConfiguration,
            sinksToWrap.Add);

        // `WriteTo.Sink()` will return the capturing configuration; this ensures chained `WriteTo` gets back
        // to the capturing sink configuration, enabling `WriteTo.X().WriteTo.Y()`.
        capturingConfiguration.WriteTo = capturingLoggerSinkConfiguration;

        configureWrappedSink(capturingLoggerSinkConfiguration);

        if(sinksToWrap.Count == 0)
        {
            return loggerSinkConfiguration._loggerConfiguration;
        }

        var enclosed = sinksToWrap.Count == 1 ?
            sinksToWrap[0] :
            new DisposingAggregateSink(sinksToWrap);

        var wrapper = wrapSink(enclosed);
        if(wrapper is not IDisposable && enclosed is IDisposable or IAsyncDisposable)
        {
            wrapper = new DisposeDelegatingSink(wrapper, enclosed as IDisposable, enclosed as IAsyncDisposable);
        }

        return loggerSinkConfiguration.Sink(wrapper, restrictedToMinimumLevel, levelSwitch);
    }
}
