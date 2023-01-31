namespace OneI.Logable;

using OneI.Logable.Templates;

public struct LoggerMessageContext
{
    internal LoggerMessageContext(
        LogLevel level,
        string message,
        Exception? exception,
        string filePath,
        string memberName,
        int line)
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
    public string Message { get; private set; }
    public readonly Exception? Exception { get; }
    public readonly string File { get; }
    public readonly string Member { get; }
    public readonly int Line { get; }

    public PropertyDictionary Properties
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    public LoggerMessageContext AddProperty<T>(string name, T value)
    {
        _ = Check.NotNull(name);

        Properties.Add(name, new(value));

        return this;
    }

    public LoggerMessageContext AddOrUpdateProperty<T>(string name, T value)
    {
        _ = Check.NotNull(name);

        Properties.Add(name, new(value));

        return this;
    }

    public LoggerMessageContext AppendText(scoped ReadOnlySpan<char> text)
    {
        if(!text.IsEmpty)
        {
            Message = string.Concat(Message, text);
        }

        return this;
    }

    public override int GetHashCode()
    {
        return Message.GetHashCode();
    }
}
