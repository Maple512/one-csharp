namespace OneI.Logable;

using Middlewares;
using Templates;

internal class Logger : ILogger
{
    internal readonly LogLevelMap _levelMap;
    internal ILoggerMiddleware[] _middlewares;
    internal readonly ILoggerSink[] _sinks;
    internal readonly TemplateProvider _templateProvider;

    internal Logger(
        ILoggerMiddleware[] middleware,
        ILoggerSink[] sinks,
        LogLevelMap levelMap,
        TemplateProvider templateProvider)
    {
        _middlewares = middleware;
        _sinks = sinks;
        _levelMap = levelMap;
        _templateProvider = templateProvider;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    public ILogger ForContext<TValue>(string name, TValue value)
    {
        var middleware = new PropertyMiddleware<TValue>(name, value, true);

        if(name.AsSpan().Equals(LoggerConstants.PropertyNames.SourceContext, StringComparison.InvariantCulture)
            && value is string sourceContext)
        {
            var range = _levelMap.GetEffectiveLevel(sourceContext);

            return ForContext(configure =>
            {
                configure.Use(middleware)
                .Level.Use(range.Minimum, range.Maximum);
            });
        }

        return ForContext(configure =>
        {
            configure.Use(middleware);
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

    #region Write

    public void Write(LogLevel level, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, level, null, message, default, file, member, line);
    }

    public void Write(LogLevel level, Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, level, exception, message, default, file, member, line);
    }

    public void Write(LogLevel level, string message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, level, null, message, default);
    }

    public void Write(LogLevel level, Exception exception, string message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, level, exception, message, default);
    }

    #endregion Write

    #region Verbose

    public void Verbose(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Verbose, null, message, default, file, member, line);
    }

    public void Verbose(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Verbose, exception, message, default, file, member, line);
    }

    public void Verbose(string message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Verbose, null, message, default);
    }

    public void Verbose(Exception exception, string message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Verbose, exception, message, default);
    }

    #endregion Verbose

    #region Debug

    public void Debug(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Debug, null, message, default, file, member, line);
    }

    public void Debug(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Debug, exception, message, default, file, member, line);
    }

    public void Debug(string message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Debug, null, message, default);
    }

    public void Debug(Exception exception, string message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Debug, exception, message, default);
    }

    #endregion Debug

    #region Information

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Information(in string message, [CallerFilePath] in string? file = null, [CallerMemberName] in string? member = null, [CallerLineNumber] in int line = 0)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Information, null, message, default, file, member, line);
    }

    public void Information(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Information, exception, message, default, file, member, line);
    }

    public void Information(string message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Information, null, message, default);
    }

    public void Information(Exception exception, string message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Information, exception, message, default);
    }

    #endregion Information

    #region Warning

    public void Warning(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Warning, null, message, default, file, member, line);
    }

    public void Warning(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Warning, exception, message, default, file, member, line);
    }

    public void Warning(string message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Warning, null, message, default);
    }

    public void Warning(Exception exception, string message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Warning, exception, message, default);
    }

    #endregion Warning

    #region Error

    public void Error(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Error, null, message, default, file, member, line);
    }

    public void Error(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Error, exception, message, default, file, member, line);
    }

    public void Error(string message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Error, null, message, default);
    }

    public void Error(Exception exception, string message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Error, exception, message, default);
    }

    #endregion Error

    #region Fatal

    public void Fatal(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Fatal, null, message, default, file, member, line);
    }

    public void Fatal(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Fatal, exception, message, default, file, member, line);
    }

    public void Fatal(string message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Fatal, null, message, default);
    }

    public void Fatal(Exception exception, string message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Fatal, exception, message, default);
    }

    #endregion Fatal

    private DisposeAction<LoggerScope> CreateScope(params ILoggerMiddleware[] middlewares)
    {
        if(middlewares.Length == 0)
        {
            return DisposeAction<LoggerScope>.Nullable;
        }

        var count = middlewares.Length + _middlewares.Length;
        var newMiddlewares = new Span<ILoggerMiddleware>(new ILoggerMiddleware[count]);

        Debugger.Break();
        if(_middlewares is { Length: > 0 })
        {
            _middlewares.CopyTo(newMiddlewares);
        }

        if(middlewares is { Length: > 0 })
        {
            middlewares.CopyTo(newMiddlewares[_middlewares.Length..]);
        }

        var scope = new LoggerScope(_middlewares);

        _middlewares = newMiddlewares.ToArray();

        return new(state => _middlewares = state.Middlewares, scope);
    }

    private void Dispatch(LoggerMessageContext context)
    {
        foreach(var item in _middlewares)
        {
            item.Invoke(context);
        }

        var enumerator = _templateProvider.GetTemplate(context);

        var loggerContext = new LoggerContext(context, enumerator.Template, enumerator.Message);

        List<Exception>? exceptions = null;

        for(var i = 0; i < _sinks.Length; i++)
        {
            try
            {
                _sinks[i].Invoke(loggerContext);
            }
            catch(Exception ex)
            {
                exceptions ??= new List<Exception>(_sinks.Length);
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
        foreach(var item in _sinks.OfType<IDisposable>())
        {
            item.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach(var item in _sinks.OfType<IAsyncDisposable>())
        {
            await item.DisposeAsync();
        }
    }
}
