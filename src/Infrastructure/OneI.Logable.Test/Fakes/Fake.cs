namespace OneI.Logable.Fakes;

using System.IO;
using OneI.Logable;

using static LoggerConstants.PropertyNames;

public static class Fake
{
    private const string Template = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}";
    public const string ErrorTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} {SourceContext} [{Level}] {Message}{NewLine}{Exception'4}{NewLine}{FilePath'4}#L{LineNumber}@{MemberName}{NewLine}";

    public static ILogger CreateLogger(
        string? template = null,
        Action<ILoggerConfiguration>? logger = null,
        Action<LogFileOptions>? file = null)
    {
        var path = Path.Combine(TestTools.GetCSProjectDirecoty()!, "Logs", $"{{{FileNameWithoutExtension}}}@{{{Member}}}.txt");

        var configuration = new LoggerConfiguration()
            .Template.Default(template ?? Template)
            .Sink.File(path, file);

        logger?.Invoke(configuration);

        return configuration.CreateLogger();
    }
}
