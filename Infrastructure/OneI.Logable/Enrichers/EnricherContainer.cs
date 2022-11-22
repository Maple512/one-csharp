namespace OneI.Logable.Enrichers;

using System;
using System.Linq;
using System.Threading;
using OneI.Logable.Properties;

public static class EnricherContainer
{
    private static readonly AsyncLocal<EnricherStack?> _stack = new();

    private static EnricherStack? Stack
    {
        get => _stack.Value;
        set => _stack.Value = value;
    }

    public static IDisposable Push(params ILoggerEnricher[] enrichers)
    {
        var stack = GetOrCreateEnricherStack();

        var bookmark = new StackBookmark(stack);

        for(var i = 0; i < enrichers.Length; i++)
        {
            stack = stack.Push(enrichers[i]);
        }

        Stack = stack;

        return bookmark;
    }

    private static EnricherStack GetOrCreateEnricherStack()
    {
        var enrichers = Stack;

        if(enrichers == null)
        {
            enrichers = EnricherStack.Empty;

            Stack = enrichers;
        }

        return enrichers;
    }

    public static ILoggerEnricher Close()
    {
        var stack = GetOrCreateEnricherStack();

        return new SafeAggregateEnricher(stack.ToList());
    }

    public static IDisposable Suspend()
    {
        var stack = GetOrCreateEnricherStack();
        var bookmark = new StackBookmark(stack);

        Stack = EnricherStack.Empty;

        return bookmark;
    }

    public static void Reset()
    {
        if(Stack != null
            && Stack != EnricherStack.Empty)
        {
            Stack = EnricherStack.Empty;
        }
    }

    internal static void Enrich(LoggerContext context, IPropertyFactory propertyFactory)
    {
        var stack = Stack;
        if(stack == null
            || stack == EnricherStack.Empty)
        {
            return;
        }

        foreach(var item in stack)
        {
            item.Enrich(context, propertyFactory);
        }
    }

    private sealed class StackBookmark : IDisposable
    {
        private readonly EnricherStack _stack;

        public StackBookmark(EnricherStack stack)
        {
            _stack = stack;
        }

        public void Dispose()
        {
            Stack = _stack;

            GC.SuppressFinalize(this);
        }
    }
}
