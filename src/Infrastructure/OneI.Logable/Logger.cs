namespace OneI.Logable;

using OneI.Logable.Internal;
using OneI.Logable.Middlewares;
using OneI.Logable.Templates;

[StackTraceHidden]
internal class Logger : ILogger
{
    internal readonly LogLevelMap _levelMap;
    internal readonly ILoggerSink[] _sinks;
    internal readonly TemplateProvider _templateProvider;
    internal ILoggerMiddleware[] _middlewares;

    internal Logger(ILoggerMiddleware[] middleware, ILoggerSink[] sinks, LogLevelMap levelMap, TemplateProvider templateProvider)
    {
        _middlewares = middleware;
        _sinks = sinks;
        _levelMap = levelMap;
        _templateProvider = templateProvider;
    }

    public void Write(ref LoggerMessageContext context, ref PropertyDictionary properties)
    {
        if(IsEnable(context.Level))
        {
            Dispatch(ref context, ref properties);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsEnable(LogLevel level) => _levelMap.IsEnabled(level);

    public void Dispose()
    {
        for(var i = 0; i < _sinks.Length; i++)
        {
            (_sinks[i] as IDisposable)?.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        for(var i = 0; i < _sinks.Length; i++)
        {
            if(_sinks[i] is IAsyncDisposable disposable)
            {
                await disposable.DisposeAsync();
            }
        }
    }

    private void Dispatch(ref LoggerMessageContext context, ref PropertyDictionary properties)
    {
        var template = _templateProvider.GetTemplate(ref context);

        List<Exception>? exceptions = null;

        for(var i = 0; i < _middlewares.Length; i++)
        {
            try
            {
                _middlewares[i].Invoke(context, ref properties);
            }
            catch(Exception ex)
            {
                exceptions ??= new List<Exception>(_sinks.Length + _middlewares.Length);
                exceptions.Add(ex);
            }
        }

        var loggerContext = new LoggerContext(ref template, ref properties, ref context);

        for(var i = 0; i < _sinks.Length; i++)
        {
            var sink = _sinks[i];
            try
            {
                sink.Invoke(loggerContext);
            }
            catch(Exception ex)
            {
                exceptions ??= new List<Exception>(_sinks.Length);
                exceptions.Add(ex);
            }
        }

        if(exceptions is { Count: > 0, })
        {
            throw new AggregateException(exceptions);
        }
    }

    #region For Context

    public ILogger ForContext(Action<ILoggerConfiguration> configure)
    {
        var configuration = new LoggerConfiguration
        {
            _defaultTemplate = _templateProvider._default
        };

        configuration._templateProviders.AddRange(_templateProvider._providers);

        configure.Invoke(configuration);

        return configuration.CreateWithLogger(this);
    }

    public ILogger ForContext(string name, object value)
    {
        var middleware = new PropertyMiddleware(name, value, true);

        if(name.AsSpan().Equals(LoggerConstants.Propertys.SourceContext, StringComparison.InvariantCulture)
           && value is string sourceContext)
        {
            var range = _levelMap.GetEffectiveLevel(sourceContext);

            return ForContext(configure =>
            {
                _ = configure.With(middleware)
                             .Level.Use(range.Minimum, range.Maximum);
            });
        }

        return ForContext(configure =>
        {
            _ = configure.With(middleware);
        });
    }

    public ILogger ForContext(params ILoggerMiddleware[] middlewares)
        => ForContext(configure =>
        {
            _ = configure.With(new AggregateMiddleware(middlewares));
        });

    public ILogger ForContext<T>()
    {
        var typename = OneIReflectionExtensions.GetTypeDisplayName(typeof(T), true, false, false, '.');

        return ForContext(typename);
    }

    public ILogger ForContext(string sourceContext)
    {
        Check.ThrowIfNull(sourceContext);

        return ForContext(LoggerConstants.Propertys.SourceContext, sourceContext);
    }

    #endregion

    #region Scope

    public IDisposable BeginScope(params ILoggerMiddleware[] middlewares) => CreateScope(middlewares);

    public IAsyncDisposable BeginScopeAsync(params ILoggerMiddleware[] middlewares) => CreateScope(middlewares);

    private DisposeAction<LoggerScope> CreateScope(params ILoggerMiddleware[] middlewares)
    {
        if(middlewares.Length == 0)
        {
            return DisposeAction<LoggerScope>.Nullable;
        }

        var count = middlewares.Length + _middlewares.Length;
        var newMiddlewares = new Span<ILoggerMiddleware>(new ILoggerMiddleware[count]);

        Debugger.Break();
        if(_middlewares is { Length: > 0, })
        {
            _middlewares.CopyTo(newMiddlewares);
        }

        if(middlewares is { Length: > 0, })
        {
            middlewares.CopyTo(newMiddlewares[_middlewares.Length..]);
        }

        var scope = new LoggerScope(_middlewares);

        _middlewares = newMiddlewares.ToArray();

        return new(state => _middlewares = state.Middlewares, scope);
    }

    public IDisposable BeginScope(string name, object value)
       => BeginScope(new PropertyMiddleware(name, value));

    public IAsyncDisposable BeginScopeAsync(string name, object value)
        => BeginScopeAsync(new PropertyMiddleware(name, value));

    #endregion
}
