namespace OneI.Logable.Sinks;

using System;
using System.Threading.Tasks;

/// <summary>
/// 受限的
/// </summary>
public class RestrictedSink : ILoggerSink, IDisposable, IAsyncDisposable
{
    private readonly ILoggerSink _next;
    private readonly LogLevelSwitch _switch;

    public RestrictedSink(LogLevelSwitch levelSwitch, ILoggerSink next)
    {
        _switch = levelSwitch;
        _next = next;
    }

    public void Emit(LoggerContext context)
    {
        if(context.Level >= _switch.MinimumLevel)
        {
            _next.Emit(context);
        }
    }

    public void Dispose()
    {
        (_next as IDisposable)?.Dispose();

        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        return (_next as IAsyncDisposable)?.DisposeAsync() ?? ValueTask.CompletedTask;
    }
}
