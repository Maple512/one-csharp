namespace OneI.Logable.Enrichers;

using System;
using System.Threading.Tasks;
using OneI.Logable.Properties;

public class ConditionalEnricher : ILoggerEnricher, IDisposable, IAsyncDisposable
{
    private readonly ILoggerEnricher _next;
    private readonly Func<LoggerContext, bool> _condition;

    public ConditionalEnricher(ILoggerEnricher next, Func<LoggerContext, bool> condition)
    {
        _next = next;
        _condition = condition;
    }

    public void Enrich(LoggerContext context, IPropertyFactory propertyFactory)
    {
        if(_condition(context))
        {
            _next.Enrich(context, propertyFactory);
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

        return (_next as IAsyncDisposable)?.DisposeAsync()
            ?? ValueTask.CompletedTask;
    }
}
