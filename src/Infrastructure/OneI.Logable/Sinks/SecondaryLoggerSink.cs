namespace OneI.Logable.Sinks;

using System.Threading.Tasks;

public class SecondaryLoggerSink : ILoggerSink, IDisposable, IAsyncDisposable
{
    private readonly ILogger _secondaryLogger;
    private readonly bool _autoDispose;

    public SecondaryLoggerSink(ILogger secondaryLogger, bool autoDispose = false)
    {
        _secondaryLogger = secondaryLogger;
        _autoDispose = autoDispose;
    }

    public void Invoke(in LoggerContext context)
    {
        _secondaryLogger.Write(context.MessageContext);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        if(_autoDispose && _secondaryLogger is IDisposable d)
        {
            d.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if(_autoDispose && _secondaryLogger is IAsyncDisposable ad)
        {
            GC.SuppressFinalize(this);

            await ad.DisposeAsync();
            return;
        }

        Dispose();

        await ValueTask.CompletedTask;
    }
}
