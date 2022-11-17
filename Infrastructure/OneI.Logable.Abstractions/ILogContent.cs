namespace OneI.Logable;

using System;

public interface ILogContent
{
    DateTimeOffset Timestamp { get; }

    LogLevel Level { get; }

    string? Text { get; }

    Exception? Exception { get; }

    object?[]? Parameters { get; }
}
