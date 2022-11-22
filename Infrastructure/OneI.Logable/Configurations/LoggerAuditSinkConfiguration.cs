namespace OneI.Logable.Configurations;

using System;

public class LoggerAuditSinkConfiguration
{
    private readonly LoggerSinkConfiguration _sinkConfiguration;

    public LoggerAuditSinkConfiguration(
        LoggerConfiguration loggerConfiguration,
        Action<ILoggerSink> sinkAction)
    {
        _sinkConfiguration = new LoggerSinkConfiguration(loggerConfiguration, sinkAction);
    }

    public LoggerConfiguration Sink(
        ILoggerSink sink,
        LogLevel restrictedToMinimumLevel = LogLevelSwitch.Minimum,
        LogLevelSwitch? levelSwitch = null)
    {
        return _sinkConfiguration.Sink(sink, restrictedToMinimumLevel, levelSwitch);
    }

    public LoggerConfiguration Sink<TSink>(
        LogLevel restrictedToMinimumLevel = LogLevelSwitch.Minimum,
        LogLevelSwitch? levelSwitch = null)
        where TSink : ILoggerSink, new()
    {
        return _sinkConfiguration.Sink<TSink>(restrictedToMinimumLevel, levelSwitch);
    }

    public LoggerConfiguration Logger(
        Action<LoggerConfiguration> configureLogger,
        LogLevel restrictedToMinimumLevel = LogLevelSwitch.Minimum,
        LogLevelSwitch? levelSwitch = null)
    {
        return _sinkConfiguration.Logger(configureLogger, restrictedToMinimumLevel, levelSwitch);
    }

    public LoggerConfiguration Logger(
        ILogger logger,
        LogLevel restrictedToMinimumLevel = LogLevelSwitch.Minimum)
    {
        return _sinkConfiguration.Logger(logger, restrictedToMinimumLevel);
    }
}
