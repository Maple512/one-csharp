namespace OneI.Logable;

using OneI.Logable.Middlewares;
using OneI.Logable.Templating.Properties;

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

    public static void PackageWrite(
        in ILogger logger,
        in LogLevel level,
        in Exception? exception,
        in ReadOnlyMemory<char> message,
        List<PropertyValue>? propertyValues = null,
        [CallerFilePath] string? filePath = null,
        [CallerMemberName] string? memberName = null,
        [CallerLineNumber] int? lineNumber = null)
    {
        propertyValues ??= new List<PropertyValue>(0);

        //var template = TextTemplate.Create(message);

        //var propertyTokens = template.PropertyTokens;

        var count = propertyValues.Count;
        //var length = Math.Max(propertyTokens.Count, count);

        var properties = new Dictionary<string, PropertyValue>(propertyValues.Count);

        for(var i = 0; i < propertyValues.Count; i++)
        {
            var name = $"__{i}";
            var index = i;
            //if(i < propertyTokens.Count)
            //{
            //    var token = propertyTokens[i];

            //    name = token.Name;

            //    index = token.ParameterIndex ?? i;
            //}

            //if(count > index)
            {
                properties.Add(name, propertyValues[i]);
            }
        }

        logger.Write(new(level, message, exception, properties, filePath, memberName, lineNumber));
    }
}
