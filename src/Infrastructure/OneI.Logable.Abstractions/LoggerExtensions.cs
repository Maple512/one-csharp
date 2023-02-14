namespace OneI.Logable;

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

    public static ILogger ForContext(this ILogger logger, params ILoggerMiddleware[] middlewares)
        => logger.ForContext(configure =>
        {
            _ = configure.With(new AggregateMiddleware(middlewares));
        });

    public static ILogger ForContext< [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TSourceContext>(this ILogger logger)
        => logger.ForContext(typeof(TSourceContext).FullName!);

    public static ILogger ForContext(this ILogger logger, string sourceContext)
        => logger.ForContext(LoggerConstants.Propertys.SourceContext, sourceContext);

    #endregion

    #region Begin Scope

    public static IDisposable BeginScope<T>(this ILogger logger, string name, T value)
        => logger.BeginScope(new PropertyMiddleware<T>(name, value));

    public static IAsyncDisposable BeginScopeAsync<T>(this ILogger logger, string name, T value)
        => logger.BeginScopeAsync(new PropertyMiddleware<T>(name, value));

    #endregion
}
