namespace OneI.Logable;

using System;
using System.Collections.Generic;
using OneI.Logable.Templating;

public static class LoggerExtensions
{
    public static void Write(
        this ILogger logger,
        LogLevel level,
        string message,
        Exception? exception = null,
        List<Property>? properties = null)
    {
        var template = TextParser.Parse(message);

        properties ??= new List<Property>();

        var context = new LoggerContext(
            level,
            template,
            exception,
            properties);

        logger.Write(context);
    }
}
