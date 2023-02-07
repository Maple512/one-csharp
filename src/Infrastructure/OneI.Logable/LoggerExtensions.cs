namespace OneI.Logable;

using System.ComponentModel;
using OneI.Logable.Middlewares;
using OneI.Logable.Templates;

[StackTraceHidden]
public static class LoggerExtensions
{
    public static void WriteCore(
        ILogger logger,
        LogLevel level,
        Exception? exception,
        string? message,
        ref PropertyDictionary properties,
        string? file = null,
        string? member = null,
        int line = 0)
    {
        var context = new LoggerMessageContext(
            level,
            message,
            exception,
            file!,
            member!,
            line);

        logger.Write(ref context, ref properties);
    }

    #region ForContext

    /// <summary>
    ///     在现有<see cref="ILogger" />的基础上，添加指定的<see cref="ILoggerMiddleware" />
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="middlewares"></param>
    /// <returns></returns>
    public static ILogger ForContext(this ILogger logger, params ILoggerMiddleware[] middlewares)
        => logger.ForContext(configure =>
        {
            _ = configure.With(new AggregateMiddleware(middlewares));
        });

    /// <summary>
    ///     在现有<see cref="ILogger" />的基础上，添加指定的<see cref="ILoggerMiddleware" />
    ///     <para>将<see cref="LoggerConstants.Propertys.SourceContext" />属性设置为指定类型的<see cref="Type.FullName" /></para>
    /// </summary>
    /// <typeparam name="TSourceContext"></typeparam>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static ILogger ForContext<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)]
    TSourceContext>(this ILogger logger)
        => logger.ForContext(typeof(TSourceContext).FullName!);

    /// <summary>
    ///     在现有<see cref="ILogger" />的基础上，添加指定的<see cref="ILoggerMiddleware" />
    ///     <para>将<see cref="LoggerConstants.Propertys.SourceContext" />属性设置为指定的参数<paramref name="sourceContext" /></para>
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="sourceContext"></param>
    /// <returns></returns>
    public static ILogger ForContext(this ILogger logger, string sourceContext)
        => logger.ForContext(LoggerConstants.Propertys.SourceContext, sourceContext);

    #endregion

    #region Begin Scope

    /// <summary>
    ///     在现有<see cref="ILogger" />的基础上，添加指定的属性
    ///     <para>会在释放之后还原</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="logger"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IDisposable BeginScope<T>(this ILogger logger, string name, T value)
        => logger.BeginScope(new PropertyMiddleware<T>(name, value));

    /// <summary>
    ///     在现有<see cref="ILogger" />的基础上，添加指定的属性
    ///     <para>会在释放之后还原</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="logger"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IAsyncDisposable BeginScopeAsync<T>(this ILogger logger, string name, T value)
        => logger.BeginScopeAsync(new PropertyMiddleware<T>(name, value));

    #endregion
}
