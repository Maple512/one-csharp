namespace OneI.Logable;

[MSLogging.ProviderAlias("Serilog")]
internal class LogableLoggerProvider : MSLogging.ILoggerProvider, ILoggerMiddleware
{
    ILogger _logger;

    public LogableLoggerProvider(ILogger logger)
    {
        //_logger = logger.ForContext(new[] { this });
    }

    public MSLogging.ILogger CreateLogger(string categoryName)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public void Invoke(in LoggerMessageContext context)
    {
        throw new NotImplementedException();
    }
}
