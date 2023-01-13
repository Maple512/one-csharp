namespace OneI.Logable;

public readonly struct ReadOnlyMessageContext
{
    internal ReadOnlyMessageContext(
        DateTimeOffset timestamp,
        LogLevel level,
        Exception? exception,
        string? file,
        string? fileName,
        string? fileNameWithoutExtension,
        string? fileExtension,
        string? member,
        int? line)
    {
        Timestamp = timestamp;
        Level = level;
        Exception = exception;
        File = file;
        FileName = fileName;
        FileNameWithoutExtension = fileNameWithoutExtension;
        FileExtension = fileExtension;
        Member = member;
        Line = line;
    }

    public DateTimeOffset Timestamp { get; }

    public LogLevel Level { get; }

    public Exception? Exception { get; }
    public string? File { get; }


    public string? FileName { get; }

    public string? FileNameWithoutExtension { get; }

    public string? FileExtension { get; }


    public string? Member { get; }

    public int? Line { get; }
}
