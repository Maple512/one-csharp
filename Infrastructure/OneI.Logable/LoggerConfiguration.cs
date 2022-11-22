namespace OneI.Logable;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OneI.Logable.Configurations;
using OneI.Logable.Enrichers;
using OneI.Logable.Filters;
using OneI.Logable.Internal;
using OneI.Logable.Properties;
using OneI.Logable.Properties.Policies;
using OneI.Logable.Sinks;

public class LoggerConfiguration
{
    private readonly Dictionary<string, LogLevelSwitch> _overrides = new();
    private readonly List<ILoggerSink> _auditSinks = new();
    private readonly List<ILoggerSink> _sinks = new();
    private readonly List<ILoggerEnricher> _enrichers = new();
    private readonly List<ILoggerFilter> _filters = new();
    private readonly List<Type> _scalarTypes = new();
    private readonly List<IDestructuringPolicy> _destructuringPolicies = new();

    private LogLevel _minimumLevel = LogLevel.Information;
    private LogLevelSwitch? _levelSwitch;
    private int _maximumDestructuringDepth = 10;
    private int _maximumStringLength = int.MaxValue;
    private int _maximumCollectionCount = int.MaxValue;
    private bool _loggerCreated;

    public LoggerConfiguration()
    {
        WriteTo = new LoggerSinkConfiguration(this, _sinks.Add);
        Enrich = new(this, _enrichers.Add);
    }

    public LoggerEnrichmentConfiguration Enrich { get; internal set; }

    public LoggerSinkConfiguration WriteTo { get; internal set; }

    public LoggerMinimumLevelConfiguration MinimumLevel
    {
        get
        {
            return new LoggerMinimumLevelConfiguration(
                this,
                l =>
                {
                    _minimumLevel = l;
                    _levelSwitch = null;
                },
                levelSwitch => _levelSwitch = levelSwitch,
                (sourceName, levelSwitch) => _overrides[sourceName] = levelSwitch);
        }
    }

    public LoggerAuditSinkConfiguration Audit => new(this, _auditSinks.Add);

    public LoggerFilterConfiguration Filter => new(this, _filters.Add);

    public LoggerDestructuringConfiguration Destructure
    {
        get
        {
            return new LoggerDestructuringConfiguration(
                this,
                _scalarTypes.Add,
                _destructuringPolicies.Add,
                depth => _maximumDestructuringDepth = depth,
                length => _maximumStringLength = length,
                count => _maximumCollectionCount = count);
        }
    }

    public ILogger CreateLogger()
    {
        if(_loggerCreated)
        {
            throw new InvalidOperationException($"The {nameof(CreateLogger)}() has been called once and cannot be called again.");
        }

        _loggerCreated = true;

        ILoggerSink sink = new SafeAggregateSink(_sinks);

        var auditing = _auditSinks.Count > 0;
        if(auditing)
        {
            sink = new FilterSink(_filters, sink, auditing);
        }

        var propertyValueFactory = new PropertyValueFactory(
            _scalarTypes,
            _destructuringPolicies,
            auditing,
            _maximumCollectionCount,
            _maximumStringLength,
            _maximumDestructuringDepth);

        var enricher = _enrichers.Count switch
        {
            0 => new EmptyEnricher(),
            1 => _enrichers[0],
            _ => new SafeAggregateEnricher(_enrichers),
        };

        LevelOverrideMapper? levelOverrideMapper = null;
        if(_overrides.Count > 0)
        {
            levelOverrideMapper = new LevelOverrideMapper(_overrides, _minimumLevel, _levelSwitch);
        }

        var allSinks = _sinks.Concat(_auditSinks);
        var disposableSinks = allSinks.OfType<IDisposable>();
        var asyncDisposableSinks = allSinks.OfType<IAsyncDisposable>();

        void Dispose()
        {
            foreach(var item in disposableSinks)
            {
                item.Dispose();
            }
        }

        async ValueTask DisposeAsync()
        {
            foreach(var item in asyncDisposableSinks)
            {
                await item.DisposeAsync();
            }
        }

        return new Logger(
            _levelSwitch != null ? LogLevelSwitch.Minimum : _minimumLevel,
            _levelSwitch,
            sink,
            enricher,
            propertyValueFactory,
            propertyValueFactory,
            Dispose,
           DisposeAsync,
           levelOverrideMapper);
    }
}
