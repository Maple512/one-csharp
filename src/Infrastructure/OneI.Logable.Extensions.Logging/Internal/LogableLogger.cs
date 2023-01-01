namespace OneI.Logable.Internal;

using System;

internal class LogableLogger : MSLogging.ILogger
{
    ILogger _logger;
    LogableLoggerProvider _provider;

    public LogableLogger(LogableLoggerProvider provider, ILogger logger, string? name = null)
    {
        _provider = provider;

        if(name.NotNullOrWhiteSpace())
        {
            _logger = logger.ForContext(name);
        }
        else
        {
            _logger = logger;
        }
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return default;
    }

    public bool IsEnabled(MSLogging.LogLevel logLevel)
        => logLevel != MSLogging.LogLevel.None
        && _logger.IsEnable(LogLevelMapper.ToLogLevel(logLevel));

    public void Log<TState>(
        MSLogging.LogLevel logLevel,
        MSLogging.EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        throw new NotImplementedException();
    }
}
