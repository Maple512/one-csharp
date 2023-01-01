namespace OneI.Logable.Sinks;

using System;
using System.Threading.Tasks;

/// <summary>
/// 表示可以操作<see cref="ILogger"/>的副本
/// </summary>
public class SecondaryLoggerSink : ILoggerSink, IDisposable, IAsyncDisposable
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SecondaryLoggerSink"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public SecondaryLoggerSink(ILogger logger)
    {
        _logger = Check.NotNull(logger);
    }

    /// <summary>
    /// Invokes the.
    /// </summary>
    /// <param name="context">The context.</param>
    public void Invoke(in LoggerContext context)
    {
        _logger.Write(context.Copy());
    }

    /// <summary>
    /// Disposes the.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);

        (_logger as IDisposable)?.Dispose();
    }

    /// <summary>
    /// Disposes the async.
    /// </summary>
    /// <returns>A ValueTask.</returns>
    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        Dispose();

        if(_logger is IAsyncDisposable ad)
        {
            return ad.DisposeAsync();
        }

        return ValueTask.CompletedTask;
    }
}
