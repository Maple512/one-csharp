namespace OneI.Logable;

using OneI.Logable.Templatizations;

public interface ILogger
{
    public static readonly ILogger NullLogger = new None();

    bool IsEnable(LogLevel level) => false;

    void Write(in LoggerMessageContext context) { }

    /// <summary>
    /// 在现有<see cref="ILogger"/>的基础上，创建一个新的<see cref="ILogger"/>
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    ILogger ForContext(Action<ILoggerConfiguration> configure) => NullLogger;

    /// <summary>
    /// 在现有<see cref="ILogger"/>的基础上，创建一个新的<see cref="ILogger"/>
    /// </summary>
    /// <returns></returns>
    ILogger ForContext<TValue>(string name, TValue value, IPropertyValueFormatter<TValue?>? formatter = null) => NullLogger;

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

    struct None : ILogger { }
}
