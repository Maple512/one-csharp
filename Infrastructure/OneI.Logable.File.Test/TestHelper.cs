namespace OneI.Logable;

using System;

public static class TestHelper
{
    public static ILogger CreateLogger(
        string? path = null,
        string? template = null,
        Action<LoggerConfiguration>? configuration = null,
        [CallerFilePath] string? file = null)
    {
        if(path.IsNullOrWhiteSpace())
        {
            path = Path.Combine(Path.GetDirectoryName(file)!, "./Logs/log.txt");
        }

        var config = new LoggerConfiguration();

        configuration?.Invoke(config);

        return config
            .Sink.File(new(path, template))
            .CreateLogger();
    }
}
