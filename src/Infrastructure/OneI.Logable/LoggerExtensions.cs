namespace OneI.Logable;

using OneI.Logable.Middlewares;
using OneI.Logable.Templatizations;
using OneI.Logable.Templatizations.Tokenizations;

public static class LoggerExtensions
{
    public static IDisposable BeginScope<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TSourceContext>(this ILogger logger)
    {
        return logger.BeginScope(typeof(TSourceContext).FullName!);
    }

    public static IDisposable BeginScope(this ILogger logger, string sourceContext)
        => logger.BeginScope(LoggerConstants.PropertyNames.SourceContext, sourceContext);

    public static IDisposable BeginScope<T>(this ILogger logger, string name, T value, IPropertyValueFormatter<T>? formatter = null)
        => logger.BeginScope(new PropertyMiddleware<T>(name, value, formatter));

    public static IAsyncDisposable BeginScopeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TSourceContext>(this ILogger logger)
    {
        return logger.BeginScopeAsync(typeof(TSourceContext).FullName!);
    }

    public static IAsyncDisposable BeginScopeAsync(this ILogger logger, string sourceContext)
    {
        return logger.BeginScopeAsync(LoggerConstants.PropertyNames.SourceContext, sourceContext);
    }

    public static IAsyncDisposable BeginScopeAsync<T>(this ILogger logger, string name, T value, IPropertyValueFormatter<T>? formatter = null)
    {
        return logger.BeginScopeAsync(new PropertyMiddleware<T>(name, value, formatter));
    }

    public static void Write(
        ILogger logger,
        LogLevel level,
        Exception? exception,
        string message,
        List<PropertyValue>? propertyValues = null,
        [CallerFilePath] string? file = null,
        [CallerMemberName] string? member = null,
        [CallerLineNumber] int? line = null)
    {
        propertyValues ??= new List<PropertyValue>(0);

        var count = propertyValues.Count;

        var properties = new Dictionary<string, PropertyValue>(propertyValues.Count);

        var tokens = TemplateParser.Parse(message);

        logger.Write(new LoggerMessageContext(
            level,
            tokens,
            exception,
            file,
            member,
            line));
    }

    public static void Write(
        ILogger logger,
        LogLevel level,
        Exception? exception,
        List<ITemplateToken> tokens,
        List<PropertyValue>? propertyValues = null,
        [CallerFilePath] string? file = null,
        [CallerMemberName] string? member = null,
        [CallerLineNumber] int? line = null)
    {
        propertyValues ??= new List<PropertyValue>(0);

        var count = propertyValues.Count;

        var properties = new Dictionary<string, PropertyValue>(propertyValues.Count);

        logger.Write(new LoggerMessageContext(
            level,
            tokens,
            exception,
            file,
            member,
            line));
    }
}
