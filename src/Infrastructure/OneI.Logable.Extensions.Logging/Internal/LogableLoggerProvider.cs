namespace OneI.Logable;

using OneI.Logable.Internal;

[MSLogging.ProviderAlias("Logable")]
internal class LogableLoggerProvider : MSLogging.ILoggerProvider
{
    ILogger _logger;

    public LogableLoggerProvider(ILogger logger)
    {
        _logger = logger;
    }

    public MSLogging.ILogger CreateLogger(string categoryName)
    {
        return new LogableLogger(_logger, categoryName);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        (_logger as IDisposable)?.Dispose();
    }
}
