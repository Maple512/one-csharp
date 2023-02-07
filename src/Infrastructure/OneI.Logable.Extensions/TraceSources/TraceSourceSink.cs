namespace OneI.Logable;

using System.Diagnostics;
using Cysharp.Text;
using OneI.Logable.Templates;

internal class TraceSourceSink : ILoggerSink, IDisposable
{
    private readonly ConcurrentDictionary<string, TraceSource> _sources = new(StringComparer.OrdinalIgnoreCase);
    private readonly SourceSwitch _rootSourceSwitch;
    private readonly TraceListener? _rootTraceListener;
    private bool _disposed;

    public TraceSourceSink(TraceSourceOptions options)
    {
        _rootSourceSwitch = options.Switch;
        _rootTraceListener = options.Listener;
    }

    public void Invoke(in LoggerContext context)
    {
        var trace = GetOrAdd(context.SourceContext);

        var level = GetEventType(context.Message.Level);

        if(trace.Switch.ShouldTrace(level))
        {
            using var writer = new ZStringWriter();

            TemplateRenderHelper.Render(writer, context, null);

            trace.TraceEvent(level, context.EventId, writer.ToString());
        }
    }

    private TraceSource GetOrAdd(string traceSourceName)
    {
        if(_sources.TryGetValue(traceSourceName, out var source) == false)
        {
            _sources[traceSourceName] = source = InitializeTraceSource(traceSourceName);
        }

        return source;
    }

    private TraceSource InitializeTraceSource(string traceSourceName)
    {
        var traceSource = new TraceSource(traceSourceName);
        var parentSourceName = ParentSourceName(traceSourceName);

        if(string.IsNullOrEmpty(parentSourceName))
        {
            if(HasDefaultSwitch(traceSource))
            {
                traceSource.Switch = _rootSourceSwitch;
            }

            if(_rootTraceListener != null)
            {
                _ = traceSource.Listeners.Add(_rootTraceListener);
            }
        }
        else
        {
            if(HasDefaultListeners(traceSource))
            {
                var parentTraceSource = GetOrAdd(parentSourceName);
                traceSource.Listeners.Clear();
                traceSource.Listeners.AddRange(parentTraceSource.Listeners);
            }

            if(HasDefaultSwitch(traceSource))
            {
                var parentTraceSource = GetOrAdd(parentSourceName);
                traceSource.Switch = parentTraceSource.Switch;
            }
        }

        return traceSource;
    }

    private static string? ParentSourceName(string traceSourceName)
    {
        var indexOfLastDot = traceSourceName.LastIndexOf('.');

        return indexOfLastDot == -1 
            ? null
            : traceSourceName[..indexOfLastDot];
    }

    private static bool HasDefaultListeners(TraceSource traceSource)
    {
        return traceSource.Listeners.Count == 1 
            && traceSource.Listeners[0] is DefaultTraceListener;
    }

    private static bool HasDefaultSwitch(TraceSource traceSource)
    {
        return string.IsNullOrEmpty(traceSource.Switch.DisplayName) == string.IsNullOrEmpty(traceSource.Name) 
            && traceSource.Switch.Level == SourceLevels.Off;
    }

    private static TraceEventType GetEventType(LogLevel level) => level switch
    {
        LogLevel.Information => TraceEventType.Information,
        LogLevel.Warning => TraceEventType.Warning,
        LogLevel.Error => TraceEventType.Error,
        LogLevel.Fatal => TraceEventType.Critical,
        _ => TraceEventType.Verbose,
    };

    /// <inheritdoc />
    public void Dispose()
    {
        if(!_disposed)
        {
            _disposed = true;
            if(_rootTraceListener != null)
            {
                _rootTraceListener.Flush();
                _rootTraceListener.Dispose();
            }
        }
    }
}
