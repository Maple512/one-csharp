namespace OneI.Logable;

using System.Threading.Tasks;
using OneI.Logable.Templates;

public interface ILogger : IDisposable, IAsyncDisposable
{
    void Write(ref LoggerMessageContext context, ref PropertyDictionary properties);

    bool IsEnable(LogLevel level);

    #region For Context

    ILogger ForContext(Action<ILoggerConfiguration> configure);

    ILogger ForContext<TValue>(string name, TValue value);

    ILogger ForContext(params ILoggerMiddleware[] middlewares);

    ILogger ForContext<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>();
    
    ILogger ForContext(string context);

    #endregion

    #region Begin Scope

    IDisposable BeginScope(params ILoggerMiddleware[] middlewares);

    IAsyncDisposable BeginScopeAsync(params ILoggerMiddleware[] middlewares);

    IDisposable BeginScope<T>(string name, T value);

    IAsyncDisposable BeginScopeAsync<T>(string name, T value);

    #endregion
}

public readonly struct NoneLogger : ILogger
{
    public static readonly ILogger Instance = new NoneLogger();

    public IDisposable BeginScope(params ILoggerMiddleware[] middlewares)
    {
        return DisposeAction.Nullable;
    }

    public IDisposable BeginScope<T>(string name, T value)
    {
        return DisposeAction.Nullable;
    }

    public IAsyncDisposable BeginScopeAsync(params ILoggerMiddleware[] middlewares)
    {
        return DisposeAction.Nullable;
    }

    public IAsyncDisposable BeginScopeAsync<T>(string name, T value)
    {
        return DisposeAction.Nullable;
    }

    public void Dispose()
    {

    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public ILogger ForContext(Action<ILoggerConfiguration> configure)
    {
        return this;
    }

    public ILogger ForContext<TValue>(string name, TValue value)
    {
        return this;
    }

    public ILogger ForContext(params ILoggerMiddleware[] middlewares)
    {
        return this;
    }

    public ILogger ForContext<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TSourceContext>()
    {
        return this;
    }

    public ILogger ForContext(string sourceContext)
    {
        return this;
    }

    public bool IsEnable(LogLevel level)
    {
        return false;
    }

    public void Write(ref LoggerMessageContext context, ref PropertyDictionary properties)
    {
    }
}
