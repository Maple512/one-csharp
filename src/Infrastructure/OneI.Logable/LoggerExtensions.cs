namespace OneI.Logable;

using OneI.Logable.Middlewares;
using OneI.Logable.Templatizations;
using OneI.Logable.Templatizations.Tokenizations;

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
    /// <param name="formatter"></param>
    /// <returns></returns>
    public static IDisposable BeginScope<T>(this ILogger logger, string name, T value, IPropertyValueFormatter<T?>? formatter = null)
        => logger.BeginScope(new PropertyMiddleware<T>(name, value, formatter));

    /// <summary>
    /// 在现有<see cref="ILogger"/>的基础上，添加指定的属性
    /// <para>会在释放之后还原</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="logger"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="formatter"></param>
    /// <returns></returns>
    public static IAsyncDisposable BeginScopeAsync<T>(this ILogger logger, string name, T value, IPropertyValueFormatter<T?>? formatter = null)
    {
        return logger.BeginScopeAsync(new PropertyMiddleware<T>(name, value, formatter));
    }

    #endregion

    public static void Write(
        ILogger logger,
        LogLevel level,
        Exception? exception,
        string message,
        List<ITemplatePropertyValue>? propertyValues = null,
        [CallerFilePath] string? file = null,
        [CallerMemberName] string? member = null,
        [CallerLineNumber] int? line = null)
    {
        propertyValues ??= new List<ITemplatePropertyValue>(0);

        var properties = new PropertyCollection(propertyValues.Count + 10);

        foreach(var item in propertyValues)
        {
            properties.Add(properties.Count, item);
        }

        var tokens = _cache.GetOrAdd(message.GetHashCode(), TemplateParser.Parse(message));

        logger.Write(new LoggerMessageContext(
            level,
            tokens,
            exception,
            properties,
            file,
            member,
            line));
    }

    private static readonly Dictionary<int, List<ITemplateToken>> _cache = new(20);
}
