namespace OneI.Logable;

using OneI.Textable;
using OneI.Textable.Templating.Properties;
/// <summary>
/// The logger extensions.
/// </summary>

public static class LoggerExtensions
{
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

        var template = TemplateParser.Parse(message);
        var tokens = template.Properties;

        var count = propertyValues.Count();
        var length = Math.Max(tokens.Count, count);

        var properties = new Dictionary<string, PropertyValue>(length);

        for(var i = 0; i < length; i++)
        {
            var name = $"__{i}";
            var index = i;
            if(i < tokens.Count)
            {
                var token = tokens[i];

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
