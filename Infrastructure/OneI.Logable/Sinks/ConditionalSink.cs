namespace OneI.Logable.Sinks;

using System;
using System.Threading.Tasks;

public class ConditionalSink : ILoggerSink, IDisposable, IAsyncDisposable
{
    private readonly ILoggerSink _wrapped;
    private readonly Func<LoggerContext, bool> _condition;

    public ConditionalSink(ILoggerSink wrapped, Func<LoggerContext, bool> condition)
    {
        _wrapped = wrapped;
        _condition = condition;
    }

    public void Emit(LoggerContext context)
    {
        if(_condition(context))
        {
            _wrapped.Emit(context);
        }
    }

    public void Dispose()
    {
        if(_wrapped is IDisposable disposable)
        {
            disposable.Dispose();
        }

        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        if(_wrapped is IAsyncDisposable disposable)
        {
            return disposable.DisposeAsync();
        }

        return ValueTask.CompletedTask;
    }
}
