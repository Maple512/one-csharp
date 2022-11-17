namespace OneI.Logable;

using System;

[Serializable]
public class LogContent : ILogContent
{
    public LogContent(LogLevel level)
        : this(level, null, null, null)
    {
    }

    public LogContent(LogLevel level, string? text)
        : this(level, text, null, null)
    {
    }

    public LogContent(LogLevel level, string? text, Exception? exception)
        : this(level, text, exception, null)
    {
    }

    public LogContent(
        LogLevel level,
        string? text,
        Exception? exception = null,
        object?[]? parameters = null)
    {
        Timestamp = DateTimeOffset.UtcNow;
        Level = level;
        Text = text;
        Exception = exception;
        Parameters = parameters;
    }

    public DateTimeOffset Timestamp { get; }

    public LogLevel Level { get; }

    public string? Text { get; }

    public Exception? Exception { get; }

    public object?[]? Parameters { get; }

    
}
