namespace OneI.Logable.Sinks;

public class SecondaryLoggerSink : ILoggerSink, IDisposable, IAsyncDisposable
{
    private readonly ILogger _secondaryLogger;
    private readonly bool _autoDispose;

    public SecondaryLoggerSink(ILogger secondaryLogger, bool autoDispose = false)
    {
        _secondaryLogger = secondaryLogger;
        _autoDispose = autoDispose;
    }

    public void Invoke(LoggerContext context)
    {
        _secondaryLogger.Write(context.Context);
    }

    public void Dispose()
    {


        if(_autoDispose && _secondaryLogger is IDisposable d)
        {
            d.Dispose();
        }
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
}
