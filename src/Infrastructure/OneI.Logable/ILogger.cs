namespace OneI.Logable;

using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using OneI.Logable.Templates;

public interface ILogger : IDisposable, IAsyncDisposable
{
    void Write(ref LoggerMessageContext context, ref PropertyDictionary properties);

    bool IsEnable(LogLevel level);

    #region For Context

    /// <summary>
    /// 在现有<see cref="ILogger"/>的基础上，创建一个新的<see cref="ILogger"/>
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    ILogger ForContext(Action<ILoggerConfiguration> configure);

    /// <summary>
    /// 在现有<see cref="ILogger"/>的基础上，创建一个新的<see cref="ILogger"/>
    /// </summary>
    /// <returns></returns>
    ILogger ForContext<TValue>(string name, TValue value);

    #endregion

    #region Begin Scope

    /// <summary>
    /// 在现有<see cref="ILogger"/>的基础上，添加指定的<see cref="ILoggerMiddleware"/>
    /// <para>会在释放之后还原</para>
    /// </summary>
    /// <param name="middlewares"></param>
    /// <returns></returns>
    IDisposable BeginScope(params ILoggerMiddleware[] middlewares);

    /// <summary>
    /// 在现有<see cref="ILogger"/>的基础上，添加指定的<see cref="ILoggerMiddleware"/>
    /// <para>会在释放之后还原</para>
    /// </summary>
    /// <param name="middlewares"></param>
    /// <returns></returns>
    IAsyncDisposable BeginScopeAsync(params ILoggerMiddleware[] middlewares);

    #endregion

    #region Write

    void Write(LogLevel level, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0);

    void Write(LogLevel level, Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0);

    #endregion Write

    #region Verbose

    void Verbose(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0);

    void Verbose(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0);

    #endregion Verbose

    #region Debug

    void Debug(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0);

    void Debug(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0);

    #endregion Debug

    #region Information

    void Information(in string message, [CallerFilePath] in string? file = null, [CallerMemberName] in string? member = null, [CallerLineNumber] in int line = 0);

    void Information(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0);

    #endregion Information

    #region Warning

    void Warning(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0);

    void Warning(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0);

    #endregion Warning

    #region Error

    void Error(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0);

    void Error(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0);

    #endregion Error

    #region Fatal

    void Fatal(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0);

    void Fatal(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0);

    #endregion Fatal
}

public readonly struct NoneLogger : ILogger
{
    public static readonly ILogger Instance = new NoneLogger();

    public IDisposable BeginScope(params ILoggerMiddleware[] middlewares) => throw new NotImplementedException();
    public IAsyncDisposable BeginScopeAsync(params ILoggerMiddleware[] middlewares) => throw new NotImplementedException();
    public void Debug(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0) => throw new NotImplementedException();
    public void Debug(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0) => throw new NotImplementedException();
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
    public void Error(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0) => throw new NotImplementedException();
    public void Error(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0) => throw new NotImplementedException();
    public void Fatal(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0) => throw new NotImplementedException();
    public void Fatal(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0) => throw new NotImplementedException();
    public ILogger ForContext(Action<ILoggerConfiguration> configure) => throw new NotImplementedException();
    public ILogger ForContext<TValue>(string name, TValue value) => throw new NotImplementedException();
    public void Information(in string message, [CallerFilePath] in string? file = null, [CallerMemberName] in string? member = null, [CallerLineNumber] in int line = 0) => throw new NotImplementedException();
    public void Information(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0) => throw new NotImplementedException();
    public bool IsEnable(LogLevel level) => throw new NotImplementedException();
    public void Verbose(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0) => throw new NotImplementedException();
    public void Verbose(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0) => throw new NotImplementedException();
    public void Warning(string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0) => throw new NotImplementedException();
    public void Warning(Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0) => throw new NotImplementedException();
    public void Write(ref LoggerMessageContext context, ref PropertyDictionary properties) => throw new NotImplementedException();
    public void Write(LogLevel level, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0) => throw new NotImplementedException();
    public void Write(LogLevel level, Exception exception, string message, [CallerFilePath] string? file = null, [CallerMemberName] string? member = null, [CallerLineNumber] int line = 0) => throw new NotImplementedException();
}
