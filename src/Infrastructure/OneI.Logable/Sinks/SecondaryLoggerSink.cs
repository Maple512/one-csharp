namespace OneI.Logable.Sinks;

using System;
using System.Threading.Tasks;

/// <summary>
/// 表示可以操作<see cref="ILogger"/>的副本
/// </summary>
public class SecondaryLoggerSink : ILoggerSink, IDisposable, IAsyncDisposable
{
    private readonly ILogger _logger;

    public SecondaryLoggerSink(ILogger logger)
    {
        _logger = Check.NotNull(logger);
    }

    public void Invoke(in LoggerContext context)
    {
        _logger.Write(context.Copy());
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        (_logger as IDisposable)?.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        if(_logger is IAsyncDisposable ad)
        {
            return ad.DisposeAsync();
        }

        return ValueTask.CompletedTask;
    }
}
