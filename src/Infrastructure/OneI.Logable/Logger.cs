namespace OneI.Logable;

using Middlewares;
using Templatizations;

internal class Logger : ILogger
{
    internal readonly LogLevelMap _levelMap;
    internal ILoggerMiddleware[] _middlewares;
    internal readonly ILoggerSink[] _sinks;
    internal readonly ITemplateSelector _templateSelector;

    internal Logger(
        ILoggerMiddleware[] middleware,
        ILoggerSink[] sinks,
        LogLevelMap levelMap,
        ITemplateSelector templateSelector)
    {
        _middlewares = middleware;
        _sinks = sinks;
        _levelMap = levelMap;
        _templateSelector = templateSelector;
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

    public ILogger ForContext<TValue>(string name, TValue value, IPropertyValueFormatter<TValue?>? formatter = null)
    {
        var middleware = new PropertyMiddleware<TValue>(name, value, formatter, true);

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
        LoggerExtensions.WriteCore(this, level, null, message.AsMemory(), null, file.AsMemory(), member.AsMemory(), line);
    }

    public void Write(LogLevel level, Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, level, exception, message.AsMemory(), null, file.AsMemory(), member.AsMemory(), line);
    }

    public void Write(LogLevel level, string? message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, level, null, message.AsMemory(), null);
    }

    public void Write(LogLevel level, Exception exception, string? message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, level, exception, message.AsMemory(), null);
    }

    #endregion Write

    #region Verbose

    public void Verbose(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Verbose, null, message.AsMemory(), null, file.AsMemory(), member.AsMemory(), line);
    }

    public void Verbose(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Verbose, exception, message.AsMemory(), null, file.AsMemory(), member.AsMemory(), line);
    }

    public void Verbose(string? message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Verbose, null, message.AsMemory(), null);
    }

    public void Verbose(Exception exception, string? message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Verbose, exception, message.AsMemory(), null);
    }

    #endregion Verbose

    #region Debug

    public void Debug(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Debug, null, message.AsMemory(), null, file.AsMemory(), member.AsMemory(), line);
    }

    public void Debug(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Debug, exception, message.AsMemory(), null, file.AsMemory(), member.AsMemory(), line);
    }

    public void Debug(string? message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Debug, null, message.AsMemory(), null);
    }

    public void Debug(Exception exception, string? message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Debug, exception, message.AsMemory(), null);
    }

    #endregion Debug

    #region Information

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Information(in string message, [CallerFilePath] in string? file = null, [CallerMemberName] in string? member = null, [CallerLineNumber] in int line = 0)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Information, null, message.AsMemory(), null, file.AsMemory(), member.AsMemory(), line);
    }

    public void Information(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Information, exception, message.AsMemory(), null, file.AsMemory(), member.AsMemory(), line);
    }

    public void Information(string? message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Information, null, message.AsMemory(), null);
    }

    public void Information(Exception exception, string? message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Information, exception, message.AsMemory(), null);
    }

    #endregion Information

    #region Warning

    public void Warning(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Warning, null, message.AsMemory(), null, file.AsMemory(), member.AsMemory(), line);
    }

    public void Warning(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Warning, exception, message.AsMemory(), null, file.AsMemory(), member.AsMemory(), line);
    }

    public void Warning(string? message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Warning, null, message.AsMemory(), null);
    }

    public void Warning(Exception exception, string? message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Warning, exception, message.AsMemory(), null);
    }

    #endregion Warning

    #region Error

    public void Error(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Error, null, message.AsMemory(), null, file.AsMemory(), member.AsMemory(), line);
    }

    public void Error(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Error, exception, message.AsMemory(), null, file.AsMemory(), member.AsMemory(), line);
    }

    public void Error(string? message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Error, null, message.AsMemory(), null);
    }

    public void Error(Exception exception, string? message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Error, exception, message.AsMemory(), null);
    }

    #endregion Error

    #region Fatal

    public void Fatal(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Fatal, null, message.AsMemory(), null, file.AsMemory(), member.AsMemory(), line);
    }

    public void Fatal(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Fatal, exception, message.AsMemory(), null, file.AsMemory(), member.AsMemory(), line);
    }

    public void Fatal(string? message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Fatal, null, message.AsMemory(), null);
    }

    public void Fatal(Exception exception, string? message, params object?[] args)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Fatal, exception, message.AsMemory(), null);
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

        var tokens = _templateSelector.Select(context);

        var loggerContext = new LoggerContext(context, tokens);

        for(var i = 0; i < _sinks.Length; i++)
        {
            try
            {
                _sinks[i].Invoke(loggerContext);
            }
            catch(Exception)
            {
            }
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
