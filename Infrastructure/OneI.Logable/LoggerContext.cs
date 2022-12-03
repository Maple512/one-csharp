namespace OneI.Logable;

using System;

public class LoggerContext
{
    public DateTimeOffset Timestamp { get; }

    public LogLevel Level { get; }

    public string? Message { get; }

    public Exception? Exception { get; }

    public string? MemverName { get; }

    public string? FilePath { get; }

    public int? Line { get; }
}
