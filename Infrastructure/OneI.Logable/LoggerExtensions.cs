namespace OneI.Logable;

using OneI.Logable.Templating;
using OneI.Logable.Templating.Properties;

public static class LoggerExtensions
{
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

        var template = TextParser.Parse(message);
        var tokens = template.Properties;

        var count = propertyValues.Count();
        var length = Math.Max(tokens.Count, count);

        var properties = new List<Property>(length);

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
                properties.Add(new(name, propertyValues.ElementAt(i)));
            }
        }

        logger.Write(new(level, template, exception, properties, filePath, memberName, lineNumber));
    }
}
