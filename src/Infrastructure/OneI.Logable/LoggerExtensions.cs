namespace OneI.Logable;

using OneI.Logable.Middlewares;
using OneI.Textable;
using OneI.Textable.Templating.Properties;

/// <summary>
/// The logger extensions.
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Fors the context.
    /// </summary>
    /// <param name="logger"></param>
    /// <returns>An ILogger.</returns>
    public static ILogger ForContext<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TSourceContext>(this ILogger logger)
    {
        return logger.ForContext(typeof(TSourceContext).FullName!);
    }

    /// <summary>
    /// Fors the context.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="sourceContext">The source context.</param>
    /// <returns>An ILogger.</returns>
    public static ILogger ForContext(this ILogger logger, string sourceContext)
    {
        return logger.ForContext(new PropertyMiddleware(LoggerConstants.PropertyNames.SourceContext, PropertyValue.CreateLiteral(sourceContext)));
    }

    public static IDisposable BeginScope<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TSourceContext>(this ILogger logger)
    {
        return logger.BeginScope(typeof(TSourceContext).FullName!);
    }

    public static IDisposable BeginScope(this ILogger logger, string sourceContext)
        => logger.BeginScope(LoggerConstants.PropertyNames.SourceContext, sourceContext);

    public static IDisposable BeginScope<T>(this ILogger logger, string name, T value)
        => logger.BeginScope(new PropertyMiddleware(name, PropertyValue.CreateLiteral(value)));

    public static IAsyncDisposable BeginScopeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TSourceContext>(this ILogger logger)
    {
        return logger.BeginScopeAsync(typeof(TSourceContext).FullName!);
    }

    public static IAsyncDisposable BeginScopeAsync(this ILogger logger, string sourceContext)
    {
        return logger.BeginScopeAsync(LoggerConstants.PropertyNames.SourceContext, sourceContext);
    }

    public static IAsyncDisposable BeginScopeAsync<T>(this ILogger logger, string name, T value)
    {
        return logger.BeginScopeAsync(new PropertyMiddleware(name, PropertyValue.CreateLiteral(value)));
    }

    /// <summary>
    /// Packages the write.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="level">The level.</param>
    /// <param name="exception">The exception.</param>
    /// <param name="message">The message.</param>
    /// <param name="propertyValues">The property values.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="memberName">The member name.</param>
    /// <param name="lineNumber">The line number.</param>
    public static void PackageWrite(
        ILogger logger,
        LogLevel level,
        Exception? exception,
        string message,
        IEnumerable<PropertyValue>? propertyValues = null,
        [CallerFilePath] string? filePath = null,
        [CallerMemberName] string? memberName = null,
        [CallerLineNumber] int? lineNumber = null)
    {
        propertyValues ??= new List<PropertyValue>(0);

        var template = TextTemplate.Create(message);

        var propertyTokens = template.PropertyTokens;

        var count = propertyValues.Count();
        var length = Math.Max(propertyTokens.Count, count);

        var properties = new Dictionary<string, PropertyValue>(length);

        for(var i = 0; i < length; i++)
        {
            var name = $"__{i}";
            var index = i;
            if(i < propertyTokens.Count)
            {
                var token = propertyTokens[i];

                name = token.Name;

                index = token.ParameterIndex ?? i;
            }

            if(count > index)
            {
                properties.Add(name, propertyValues.ElementAt(i));
            }
        }

        logger.Write(new(level, template, exception, properties, filePath, memberName, lineNumber));
    }
}
