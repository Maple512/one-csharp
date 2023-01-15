namespace OneI.Logable.Internal;

using System;
using OneI.Logable.Templatizations;

internal class LogableLogger : MSLogging.ILogger
{
    public const string Scope = nameof(Scope);

    private readonly ILogger _logger;

    public LogableLogger(ILogger logger, string? name = null)
    {
        if(name is { Length: > 0 })
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
        return _logger.BeginScope(Scope, state);
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
        var level = LogLevelMapper.ToLogLevel(logLevel);

        var message = formatter.Invoke(state, exception);

        LoggerExtensions.Write(_logger, level, exception, message);
    }
}
