namespace OneI.Logable;

using System;
using System.ComponentModel;
using Middlewares;
using Templates;

public static class LoggerExtensions
{
    #region ForContext

    /// <summary>
    /// 在现有<see cref="ILogger"/>的基础上，添加指定的<see cref="ILoggerMiddleware"/>
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="middlewares"></param>
    /// <returns></returns>
    public static ILogger ForContext(this ILogger logger, params ILoggerMiddleware[] middlewares)
    {
        return logger.ForContext(configure =>
        {
            configure.Use(new AggregateMiddleware(middlewares));
        });
    }

    /// <summary>
    /// 在现有<see cref="ILogger"/>的基础上，添加指定的<see cref="ILoggerMiddleware"/>
    /// <para>将<see cref="LoggerConstants.PropertyNames.SourceContext"/>属性设置为指定类型的<see cref="Type.FullName"/></para>
    /// </summary>
    /// <typeparam name="TSourceContext"></typeparam>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static ILogger ForContext<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TSourceContext>(this ILogger logger)
    {
        return logger.ForContext(typeof(TSourceContext).FullName!);
    }

    /// <summary>
    /// 在现有<see cref="ILogger"/>的基础上，添加指定的<see cref="ILoggerMiddleware"/>
    /// <para>将<see cref="LoggerConstants.PropertyNames.SourceContext"/>属性设置为指定的参数<paramref name="sourceContext"/></para>
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="sourceContext"></param>
    /// <returns></returns>
    public static ILogger ForContext(this ILogger logger, string sourceContext)
    {
        return logger.ForContext(LoggerConstants.PropertyNames.SourceContext, sourceContext);
    }

    #endregion

    #region Begin Scope

    /// <summary>
    /// 在现有<see cref="ILogger"/>的基础上，添加指定的属性
    /// <para>会在释放之后还原</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="logger"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IDisposable BeginScope<T>(this ILogger logger, string name, T value)
        => logger.BeginScope(new PropertyMiddleware<T>(name, value));

    /// <summary>
    /// 在现有<see cref="ILogger"/>的基础上，添加指定的属性
    /// <para>会在释放之后还原</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="logger"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IAsyncDisposable BeginScopeAsync<T>(this ILogger logger, string name, T value)
    {
        return logger.BeginScopeAsync(new PropertyMiddleware<T>(name, value));
    }

    #endregion

    #region Write

    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Write(this ILogger logger, LogLevel level, string message, params object?[] args)
    {
        WriteCore(logger, level, null, message, default);
    }

    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Write(this ILogger logger, LogLevel level, Exception exception, string message, params object?[] args)
    {
        WriteCore(logger, level, exception, message, default);
    }

    #endregion Write

    #region Verbose
    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Verbose(this ILogger logger, string message, params object?[] args)
    {
        WriteCore(logger, LogLevel.Verbose, null, message, default);
    }
    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Verbose(this ILogger logger, Exception exception, string message, params object?[] args)
    {
        WriteCore(logger, LogLevel.Verbose, exception, message, default);
    }

    #endregion Verbose

    #region Debug
    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Debug(this ILogger logger, string message, params object?[] args)
    {
        WriteCore(logger, LogLevel.Debug, null, message, default);
    }
    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Debug(this ILogger logger, Exception exception, string message, params object?[] args)
    {
        WriteCore(logger, LogLevel.Debug, exception, message, default);
    }

    #endregion Debug

    #region Information
    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Information(this ILogger logger, string message, params object?[] args)
    {
        WriteCore(logger, LogLevel.Information, null, message, default);
    }
    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Information(this ILogger logger, Exception exception, string message, params object?[] args)
    {
        WriteCore(logger, LogLevel.Information, exception, message, default);
    }

    #endregion Information

    #region Warning
    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Warning(this ILogger logger, string message, params object?[] args)
    {
        WriteCore(logger, LogLevel.Warning, null, message, default);
    }
    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Warning(this ILogger logger, Exception exception, string message, params object?[] args)
    {
        WriteCore(logger, LogLevel.Warning, exception, message, default);
    }

    #endregion Warning

    #region Error
    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Error(this ILogger logger, string message, params object?[] args)
    {
        WriteCore(logger, LogLevel.Error, null, message, default);
    }
    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Error(this ILogger logger, Exception exception, string message, params object?[] args)
    {
        WriteCore(logger, LogLevel.Error, exception, message, default);
    }

    #endregion Error

    #region Fatal
    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Fatal(this ILogger logger, string message, params object?[] args)
    {
        WriteCore(logger, LogLevel.Fatal, null, message, default);
    }
    [Conditional(SharedConstants.DEBUG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Fatal(this ILogger logger, Exception exception, string message, params object?[] args)
    {
        WriteCore(logger, LogLevel.Fatal, exception, message, default);
    }

    #endregion Fatal

    public static void WriteCore(
        ILogger logger,
        LogLevel level,
        Exception? exception,
        string message,
        ValueDictionary<string, ITemplatePropertyValue> properties,
        string? file = null,
        string? member = null,
        int line = 0)
    {
        var context = new LoggerMessageContext(
            level,
            message,
            exception,
            properties,
            file!,
            member!,
            line);

        logger.Write(context);
    }
}
