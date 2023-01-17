namespace OneI.Logable;

using OneI.Logable.Templatizations;

public interface ILogger : IDisposable, IAsyncDisposable
{
    bool IsEnable(LogLevel level) => false;

    void Write(in LoggerMessageContext context) { }

    /// <summary>
    /// 在现有<see cref="ILogger"/>的基础上，创建一个新的<see cref="ILogger"/>
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    ILogger ForContext(Action<ILoggerConfiguration> configure) => NoneLogger.Instance;

    /// <summary>
    /// 在现有<see cref="ILogger"/>的基础上，创建一个新的<see cref="ILogger"/>
    /// </summary>
    /// <returns></returns>
    ILogger ForContext<TValue>(string name, TValue value, IPropertyValueFormatter<TValue?>? formatter = null) => NoneLogger.Instance;

    /// <summary>
    /// 在现有<see cref="ILogger"/>的基础上，添加指定的<see cref="ILoggerMiddleware"/>
    /// <para>会在释放之后还原</para>
    /// </summary>
    /// <param name="middlewares"></param>
    /// <returns></returns>
    IDisposable BeginScope(params ILoggerMiddleware[] middlewares)
        => DisposeAction.Nullable;

    /// <summary>
    /// 在现有<see cref="ILogger"/>的基础上，添加指定的<see cref="ILoggerMiddleware"/>
    /// <para>会在释放之后还原</para>
    /// </summary>
    /// <param name="middlewares"></param>
    /// <returns></returns>
    IAsyncDisposable BeginScopeAsync(params ILoggerMiddleware[] middlewares)
        => DisposeAction.Nullable;

    #region Write

    void Write(LogLevel level, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int? line = null)
    {
        LoggerExtensions.WriteCore(this, level, null, message, null, file, member, line);
    }
    
    void Write(LogLevel level, Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int? line = null)
    {
        LoggerExtensions.WriteCore(this, level, exception, message, null, file, member, line);
    }

    #endregion Write

    #region Verbose
    
    void Verbose(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int? line = null)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Verbose, null, message, null, file, member, line);
    }
    
    void Verbose(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int? line = null)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Verbose, exception, message, null, file, member, line);
    }

    #endregion Verbose

    #region Debug
    
    void Debug(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int? line = null)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Debug, null, message, null, file, member, line);
    }
    
    void Debug(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int? line = null)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Debug, exception, message, null, file, member, line);
    }

    #endregion Debug

    #region Information

    void Information(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int? line = null)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Information, null, message, null, file, member, line);
    }

    void Information(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int? line = null)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Information, exception, message, null, file, member, line);
    }

    #endregion Information

    #region Warning

    void Warning(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int? line = null)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Warning, null, message, null, file, member, line);
    }
    
    void Warning(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int? line = null)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Warning, exception, message, null, file, member, line);
    }

    #endregion Warning

    #region Error

    void Error(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int? line = null)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Error, null, message, null, file, member, line);
    }
    
    void Error(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int? line = null)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Error, exception, message, null, file, member, line);
    }

    #endregion Error

    #region Fatal

    void Fatal(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int? line = null)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Fatal, null, message, null, file, member, line);
    }
    
    void Fatal(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int? line = null)
    {
        LoggerExtensions.WriteCore(this, LogLevel.Fatal, exception, message, null, file, member, line);
    }

    #endregion Fatal

    
}

public readonly struct NoneLogger : ILogger
{
    public static readonly ILogger Instance = new NoneLogger();

    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
    }

    ValueTask IAsyncDisposable.DisposeAsync()
    {
        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
    }
}
