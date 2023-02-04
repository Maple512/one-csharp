namespace OneI.Logable;

using System.Threading.Tasks;
using OneI.Logable.Templates;

public interface ILogger : IDisposable, IAsyncDisposable
{
    void Write(ref LoggerMessageContext context, ref PropertyDictionary properties);

    bool IsEnable(LogLevel level);

    #region For Context

    /// <summary>
    ///     在现有<see cref="ILogger" />的基础上，创建一个新的<see cref="ILogger" />
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    ILogger ForContext(Action<ILoggerConfiguration> configure);

    /// <summary>
    ///     在现有<see cref="ILogger" />的基础上，创建一个新的<see cref="ILogger" />
    /// </summary>
    /// <returns></returns>
    ILogger ForContext<TValue>(string name, TValue value);

    #endregion

    #region Begin Scope

    /// <summary>
    ///     在现有<see cref="ILogger" />的基础上，添加指定的<see cref="ILoggerMiddleware" />
    ///     <para>会在释放之后还原</para>
    /// </summary>
    /// <param name="middlewares"></param>
    /// <returns></returns>
    IDisposable BeginScope(params ILoggerMiddleware[] middlewares);

    /// <summary>
    ///     在现有<see cref="ILogger" />的基础上，添加指定的<see cref="ILoggerMiddleware" />
    ///     <para>会在释放之后还原</para>
    /// </summary>
    /// <param name="middlewares"></param>
    /// <returns></returns>
    IAsyncDisposable BeginScopeAsync(params ILoggerMiddleware[] middlewares);

    #endregion
}

public readonly struct NoneLogger : ILogger
{
    public static readonly ILogger Instance = new NoneLogger();

    public IDisposable BeginScope(params ILoggerMiddleware[] middlewares)
    {
        throw new NotImplementedException();
    }

    public IAsyncDisposable BeginScopeAsync(params ILoggerMiddleware[] middlewares)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }

    public ILogger ForContext(Action<ILoggerConfiguration> configure)
    {
        throw new NotImplementedException();
    }

    public ILogger ForContext<TValue>(string name, TValue value)
    {
        throw new NotImplementedException();
    }

    public bool IsEnable(LogLevel level)
    {
        throw new NotImplementedException();
    }

    public void Write(ref LoggerMessageContext context, ref PropertyDictionary properties)
    {
        throw new NotImplementedException();
    }
}
