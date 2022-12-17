namespace OneI.Logable.Fakes;

using OneT.Common;

public static class Fake
{
    private const string ErrorTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} {SourceContext} [{Level}] {Message}{NewLine}{Exception'4}{NewLine}{FilePath'4}#L{LineNumber}@{MemberName}{NewLine}";

    public static ILogger CreateLogger(
        string? path = null,
        string? template = null,
        Action<LoggerConfiguration>? configuration = null,
        [CallerFilePath] string? file = null,
        [CallerMemberName] string? member = null)
    {
        if(path.IsNullOrWhiteSpace())
        {
            var name = Path.GetFileNameWithoutExtension(file);

            path = Path.Combine(TestTools.GetCSProjectDirecoty()!, $"./Logs/{name}@{member}.txt");
        }

        var config = new LoggerConfiguration();

        configuration?.Invoke(config);

        var options = new LogFileOptions(path, template);

        options.RenderWhen(c => c.Exception is not null, ErrorTemplate);

        return config
            .Sink.File(options)
            .Sink.Use(new TestAuditSink())
            .CreateLogger();
    }
}
