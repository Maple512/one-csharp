namespace OneI.Logable.Sinks;

using System;
using System.Threading.Tasks;
using OneI.Logable;

/// <summary>
/// 将日志事件转发到另一个日志记录管道。
/// 复制事件，以便在副本上执行的突变不会影响原始事件。
/// </summary>
public class SecondaryLoggerSink : ILoggerSink, IDisposable, IAsyncDisposable
{
    private readonly ILogger _logger;
    private readonly bool _attemptDispose;

    public SecondaryLoggerSink(ILogger logger, bool attemptDispose = false)
    {
        _logger = logger;
        _attemptDispose = attemptDispose;
    }

    public void Emit(LoggerContext context)
    {
        var copied = context.Copy();

        _logger.Write(copied);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        if(_attemptDispose == false)
        {
            return;
        }

        (_logger as IDisposable)?.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        if(_attemptDispose == false)
        {
            return ValueTask.CompletedTask;
        }

        return (_logger as IAsyncDisposable)?.DisposeAsync()
            ?? ValueTask.CompletedTask;
    }
}
