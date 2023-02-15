namespace OneI.Logable;

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
}
