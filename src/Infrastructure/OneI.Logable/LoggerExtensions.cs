namespace OneI.Logable;

using System.ComponentModel;
using OneI.Logable.Middlewares;
using OneI.Logable.Templates;

public static class LoggerExtensions
{
    public static void WriteCore(ILogger                  logger
                                 , LogLevel               level
                                 , Exception?             exception
                                 , string                 message
                                 , ref PropertyDictionary properties
                                 , string?                file   = null
                                 , string?                member = null
                                 , int                    line   = 0)
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
            _ = configure.Use(new AggregateMiddleware(middlewares));
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

#region Write

    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Write(this ILogger logger, LogLevel level, string message, params object?[] args)
        => throw new NotSupportedException();

    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Write(this ILogger       logger
                             , LogLevel         level
                             , Exception        exception
                             , string           message
                             , params object?[] args)
        => throw new NotSupportedException();

#endregion Write

#region Verbose

    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Verbose(this ILogger logger, string message, params object?[] args)
        => throw new NotSupportedException();

    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Verbose(this ILogger logger, Exception exception, string message, params object?[] args)
        => throw new NotSupportedException();

#endregion Verbose

#region Debug

    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Debug(this ILogger logger, string message, params object?[] args)
        => throw new NotSupportedException();

    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Debug(this ILogger logger, Exception exception, string message, params object?[] args)
        => throw new NotSupportedException();

#endregion Debug

#region Information

    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Information(this ILogger logger, string message, params object?[] args)
        => throw new NotSupportedException();

    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Information(this ILogger logger, Exception exception, string message, params object?[] args)
        => throw new NotSupportedException();

#endregion Information

#region Warning

    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Warning(this ILogger logger, string message, params object?[] args)
        => throw new NotSupportedException();

    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Warning(this ILogger logger, Exception exception, string message, params object?[] args)
        => throw new NotSupportedException();

#endregion Warning

#region Error

    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Error(this ILogger logger, string message, params object?[] args)
        => throw new NotSupportedException();

    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Error(this ILogger logger, Exception exception, string message, params object?[] args)
        => throw new NotSupportedException();

#endregion Error

#region Fatal

    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Fatal(this ILogger logger, string message, params object?[] args)
        => throw new NotSupportedException();

    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Fatal(this ILogger logger, Exception exception, string message, params object?[] args)
        => throw new NotSupportedException();

#endregion Fatal
}
