namespace OneI.Logable;

using System;
using OneI.Logable.Middlewares;
using OneI.Logable.Templatizations;

internal class Logger : ILogger
{
    internal readonly LogLevelMap _levelMap;
    internal readonly AsyncLocal<ILoggerMiddleware[]> _middlewares;
    internal readonly ILoggerSink[] _sinks;
    internal readonly ITemplateSelector _templateSelector;

    internal Logger(
        ILoggerMiddleware[] middleware,
        ILoggerSink[] sinks,
        LogLevelMap levelMap,
        ITemplateSelector templateSelector)
    {
        _middlewares = new()
        {
            Value = middleware,
        };

        _sinks = sinks;
        _levelMap = levelMap;
        _templateSelector = templateSelector;
    }

    public bool IsEnable(LogLevel level)
    {
        return _levelMap.IsEnabled(level);
    }

    public ILogger ForContext(Action<ILoggerConfiguration> configure)
    {
        var configuration = new LoggerConfiguration();

        configure.Invoke(configuration);

        return configuration.CreateWithLogger(this);
    }

    public ILogger ForContext<TValue>(string name, TValue value, IPropertyValueFormatter<TValue?>? formatter = null)
    {
        if(name.AsSpan().Equals(LoggerConstants.PropertyNames.SourceContext, StringComparison.InvariantCulture)
            && value is string sourceContext)
        {
            var range = _levelMap.GetEffectiveLevel(sourceContext);

            return ForContext(configure =>
            {
                configure.Level.Use(range.Minimum, range.Maximum);
            });
        }

        return ForContext(configure =>
        {
            configure.Use(new PropertyMiddleware<TValue>(name, value, formatter, true));
        });
    }

    public void Write(in LoggerMessageContext context)
    {
        if(IsEnable(context.Level))
        {
            Dispatch(context);
        }
    }

    public IDisposable BeginScope(params ILoggerMiddleware[] middlewares)
    {
        return CreateScope(middlewares);
    }

    public IAsyncDisposable BeginScopeAsync(params ILoggerMiddleware[] middlewares)
    {
        return CreateScope(middlewares);
    }

    private DisposeAction<LoggerScope> CreateScope(params ILoggerMiddleware[] middlewares)
    {
        if(middlewares.Length == 0)
        {
            return DisposeAction<LoggerScope>.Nullable;
        }

        var count = middlewares.Length + _middlewares.Value!.Length;
        var newMidddlwares = count == 0 ? Array.Empty<ILoggerMiddleware>() : new ILoggerMiddleware[count];

        if(_middlewares.Value is { Length: > 0 })
        {
            _middlewares.Value.CopyTo(newMidddlwares, 0);
        }

        if(middlewares is { Length: > 0 })
        {
            middlewares.CopyTo(newMidddlwares, _middlewares.Value.Length);
        }

        var scope = new LoggerScope(_middlewares.Value!);

        _middlewares.Value = newMidddlwares;

        return new((state) => _middlewares.Value = state.Middlewares, scope);
    }

    private void Dispatch(in LoggerMessageContext context)
    {
        foreach(var item in _middlewares.Value!)
        {
            try
            {
                item.Invoke(context);
            }
            catch(Exception)
            {
                throw;
            }
        }

        var exceptions = new List<Exception>(_sinks.Length);

        var tokens = _templateSelector.Select(context);

        var properties = context.GetProperties();

        var loggerContext = new LoggerContext(tokens, properties.ToList(), context);

        foreach(var sink in _sinks)
        {
            try
            {
                sink.Invoke(loggerContext);
            }
            catch(Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        if(exceptions is { Count: > 0 })
        {
            throw new AggregateException(exceptions);
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        foreach(var item in _sinks)
        {
            (item as IDisposable)?.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        foreach(var item in _sinks)
        {
            if(item is IAsyncDisposable disposable)
            {
                await disposable.DisposeAsync();
            }
        }
    }
}
