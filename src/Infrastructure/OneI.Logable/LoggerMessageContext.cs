namespace OneI.Logable;

public readonly struct LoggerMessageContext
{
    internal LoggerMessageContext(LogLevel level
                                  , string message
                                  , Exception? exception
                                  , string filePath
                                  , string memberName
                                  , int line)
    {
        Timestamp = DateTimeOffset.Now;
        Level = level;
        Message = message;
        Exception = exception;
        File = filePath;
        Member = memberName;
        Line = line;
    }

    public readonly DateTimeOffset Timestamp { get; }
    public readonly LogLevel Level { get; }
    public readonly string Message { get; }
    public readonly Exception? Exception { get; }
    public readonly string File { get; }
    public readonly string Member { get; }
    public readonly int Line { get; }

    public override int GetHashCode() => Message.GetHashCode();
}
