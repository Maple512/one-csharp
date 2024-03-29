namespace OneI.Logable.Fakes;

using static LoggerConstants.Propertys;

public static class Fake
{
    private const string Template = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}";

    public const string ErrorTemplate
        = "{Timestamp:yyyy-MM-dd HH:mm:ss} {SourceContext} [{Level}] {Message}{NewLine}{Exception'4}{NewLine}{FilePath'4}#L{LineNumber}@{MemberName}{NewLine}";

    public static ILogger CreateLogger(string? template = null
                                       , Action<ILoggerConfiguration>? logger = null
                                       , Action<LogRollFileOptions>? file = null)
    {
        var path = Path.Combine(TestTools.GetCSProjectDirecoty()!, "Logs"
                                , $"{{{FileNameWithoutExtension}}}@{{{Member}}}.txt");

        var configuration = new LoggerConfiguration(template ?? Template)
                            .Sink.RollFile(path, file);

        logger?.Invoke(configuration);

        return configuration.CreateLogger();
    }
}
