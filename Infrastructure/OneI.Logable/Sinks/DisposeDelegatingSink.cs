namespace OneI.Logable.Sinks;

using System;
using System.Threading.Tasks;

public sealed class DisposeDelegatingSink : ILoggerSink, IDisposable, IAsyncDisposable
{
    private readonly ILoggerSink _sink;
    private readonly IDisposable? _disposed;
    private readonly IAsyncDisposable? _asyncDisposable;

    public DisposeDelegatingSink(ILoggerSink sink, IDisposable? disposed, IAsyncDisposable? asyncDisposable)
    {
        _sink = sink;
        _disposed = disposed;
        _asyncDisposable = asyncDisposable;
    }

    public void Emit(LoggerContext context)
    {
        _sink.Emit(context);
    }

    public void Dispose()
    {
        _disposed?.Dispose();

        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        return _asyncDisposable?.DisposeAsync() ?? ValueTask.CompletedTask;
    }
}
