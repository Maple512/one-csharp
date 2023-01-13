namespace OneI.Logable.Internal;

using System;

internal class LogableLogger : MSLogging.ILogger
{
    ILogger _logger;

    public LogableLogger(ILogger logger, string? name = null)
    {
        if(name.NotNullOrWhiteSpace())
        {
            _logger = null; //logger.ForContext(name);
        }
        else
        {
            _logger = logger;
        }
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return _logger.BeginScope();
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
