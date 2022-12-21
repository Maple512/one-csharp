namespace OneI.Logable.Fakes;

using System.IO;
using OneT.Common;

public static class Fake
{
    public const string ErrorTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} {SourceContext} [{Level}] {Message}{NewLine}{Exception'4}{NewLine}{FilePath'4}#L{LineNumber}@{MemberName}{NewLine}";

    public static ILogger CreateLogger(
        Action<ILoggerConfiguration>? configuration = null,
        Action<LoggerFileOptions>? file = null,
        Action<LoggerSharedFileOptions>? shared = null,
        Action<LoggerRollFileOptions>? roll = null,
        [CallerFilePath] string? filePath = null,
        [CallerMemberName] string? member = null)
    {
        var name = Path.GetFileNameWithoutExtension(filePath);

        var path = Path.Combine(TestTools.GetCSProjectDirecoty()!, $"./Logs/{name}@{member}.txt");

        ILoggerConfiguration config = new LoggerConfiguration();

        configuration?.Invoke(config);

        if(file is not null)
        {
            config = config.Sink.File(path, file);
        }

        if(shared is not null)
        {
            config = config.Sink.SharedFile(path, shared);
        }

        if(roll is not null)
        {
            config = config.Sink.RollFile(path, roll);
        }

        return config.CreateLogger();
    }
}
