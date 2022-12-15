namespace OneI.Logable;

public static class LoggerTest
{
    const string ErrorTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}{NewLine}{FilePath}#L{LineNumber}@{MemberName}{NewLine}";

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

            path = Path.Combine(Path.GetDirectoryName(file)!, $"./Logs/{name}@{member}.txt");
        }

        var config = new LoggerConfiguration();

        configuration?.Invoke(config);

        var options = new LogFileOptions(path, template);

        options.RenderWhen(c => c.Exception is not null, ErrorTemplate);

        return config
            .Sink.File(options)
            .CreateLogger();
    }
}
