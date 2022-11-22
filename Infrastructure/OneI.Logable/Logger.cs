namespace OneI.Logable;

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using OneI.Logable.Internal;
using OneI.Logable.Parsing;
using OneI.Logable.Properties;

public class Logger : ILogger, ILoggerSink, IDisposable, IAsyncDisposable
{
    private readonly LevelOverrideMapper? _overrideMapper;
    private readonly Action? _disposeAction;
    private readonly Func<ValueTask>? _asyncDisposeAction;
    private readonly ILoggerEnricher _enricher;
    private readonly ILoggerSink _sink;
    private readonly LogLevel _minimumLevel;
    private readonly LogLevelSwitch? _levelSwitch;
    private readonly IPropertyFactory _propertyFactory;
    private readonly PropertyBinder _propertyBinder;

    internal Logger(
        LogLevel minimumLevel,
        LogLevelSwitch? levelSwitch,
        ILoggerSink sink,
        ILoggerEnricher enricher,
        IPropertyFactory propertyFactory,
        IPropertyValueFactory propertyValueFactory,
        Action? disposeAction,
        Func<ValueTask>? asyncDisposeAction,
        LevelOverrideMapper? overrideMapper)
    {
        _minimumLevel = minimumLevel;
        _enricher = enricher;
        _sink = sink;
        _propertyFactory = propertyFactory;
        _levelSwitch = levelSwitch;
        _disposeAction = disposeAction;
        _asyncDisposeAction = asyncDisposeAction;
        _overrideMapper = overrideMapper;

        _propertyBinder = new PropertyBinder(propertyValueFactory);
    }

    internal bool HasOverrideMap => _overrideMapper != null;

    public bool IsEnabled(LogLevel level)
    {
        if(level < _minimumLevel)
        {
            return false;
        }

        return _levelSwitch == null ||
               level >= _levelSwitch.Value.MinimumLevel;
    }

    public void Write(
        LogLevel level,
        string message,
        object?[]? parameters,
        Exception? exception = null,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int? lineNumber = null)
    {
        if(message == null
            || IsEnabled(level) != false)
        {
            return;
        }

        var tokens = TextParser.Parse(message);

        var properties = _propertyBinder.ConstructProperties(tokens, parameters);

        var context = new LoggerContext(level, exception, tokens, properties, memberName, filePath, lineNumber);

        Dispatch(context);
    }

    public void Write(in LoggerContext context)
    {
        if(context?.Text == null
            || IsEnabled(context.Level) != false)
        {
            return;
        }

        Dispatch(context);
    }

    public void Dispose()
    {
        _disposeAction?.Invoke();

        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        return _asyncDisposeAction?.Invoke() ?? ValueTask.CompletedTask;
    }

    void ILoggerSink.Emit(LoggerContext context)
    {
        Dispatch(context);
    }

    private void Dispatch(LoggerContext logEvent)
    {
        // enricher可能是一个“安全”的聚合器，但通常是空的，因此这里重复了SafeAggregateEnricher的异常处理。
        try
        {
            _enricher.Enrich(logEvent, _propertyFactory);
        }
        catch(Exception ex)
        {
            InternalLog.WriteLine("Exception {0} caught while enriching {1} with {2}.", ex, logEvent, _enricher);
        }

        _sink.Emit(logEvent);
    }
}
