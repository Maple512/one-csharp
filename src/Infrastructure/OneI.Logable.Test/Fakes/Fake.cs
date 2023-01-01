namespace OneI.Logable.Fakes;

using System.IO;
using OneT.Common;

public static class Fake
{
    public const string ErrorTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} {SourceContext} [{Level}] {Message}{NewLine}{Exception'4}{NewLine}{FilePath'4}#L{LineNumber}@{MemberName}{NewLine}";

    public static ILogger CreateLogger(
        Action<ILoggerConfiguration>? logger = null,
        Action<LoggerFileOptions>? file = null,
        Action<LoggerSharedFileOptions>? shared = null,
        Action<LoggerRollFileOptions>? roll = null,
        [CallerFilePath] string? filePath = null,
        [CallerMemberName] string? member = null)
    {
        var name = Path.GetFileNameWithoutExtension(filePath);

        var path = Path.Combine(TestTools.GetCSProjectDirecoty()!, $"./Logs/{name}@{member}.txt");

        ILoggerConfiguration configuration = new LoggerConfiguration();

        logger?.Invoke(configuration);

        if(file is not null)
        {
            configuration = configuration.Sink.File(path, file);
        }

        if(shared is not null)
        {
            configuration = configuration.Sink.SharedFile(path, shared);
        }

        if(roll is not null)
        {
            configuration = configuration.Sink.RollFile(path, roll);
        }

        return configuration.CreateLogger();
    }
}
