namespace OneI.Logable.Sinks;

public class SecondaryLoggerSink : ILoggerSink, IDisposable, IAsyncDisposable
{
    private readonly bool    _autoDispose;
    private readonly ILogger _secondaryLogger;

    public SecondaryLoggerSink(ILogger secondaryLogger, bool autoDispose = false)
    {
        _secondaryLogger = secondaryLogger;
        _autoDispose     = autoDispose;
    }

    public async ValueTask DisposeAsync()
    {
        if(_autoDispose && _secondaryLogger is IAsyncDisposable ad)
        {
            await ad.DisposeAsync();
            return;
        }

        Dispose();

        await ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        if(_autoDispose && _secondaryLogger is IDisposable d)
        {
            d.Dispose();
        }
    }

    public void Invoke(in LoggerContext context)
    {
        var message    = context.Message;
        var properties = context.Properties;

        _secondaryLogger.Write(ref message, ref properties);
    }
}
