namespace OneI.Logable;

using OneI.Logable.Templates;
using Templates;

public struct LoggerMessageContext
{
    private string _message;

    internal LoggerMessageContext(
        LogLevel level,
        string message,
        in Exception? exception,
        PropertyDictionary<string, ITemplatePropertyValue> properties,
        string filePath,
        string memberName,
        int line)
    {
        Timestamp = DateTimeOffset.Now;
        Level = level;
        _message = message;
        Properties = properties;
        Exception = exception;
        File = filePath;
        Member = memberName;
        Line = line;
    }

    public readonly DateTimeOffset Timestamp { get; }
    public readonly LogLevel Level { get; }
    public readonly string Message => _message;
    public readonly Exception? Exception { get; }
    public readonly string File { get; }
    public readonly string Member { get; }
    public readonly int Line { get; }

    public PropertyDictionary<string, ITemplatePropertyValue> Properties
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    /// <inheritdoc cref="PropertyCollection.Add(string, ITemplatePropertyValue)"/>
    public LoggerMessageContext AddProperty<T>(string name, T value)
    {
        Check.NotNull(name);

        Properties.Add(name, new LiteralValue<T>(value));

        return this;
    }

    /// <inheritdoc cref="PropertyCollection.AddOrUpdate(string, ITemplatePropertyValue)"/>
    public LoggerMessageContext AddOrUpdateProperty<T>(string name, T value)
    {
        Check.NotNull(name);

        Properties.Add(name, new LiteralValue<T>(value));

        return this;
    }

    public LoggerMessageContext AppendText(scoped ReadOnlySpan<char> text)
    {
        if(!text.IsEmpty)
        {
            _message = string.Concat(_message, text);
        }

        return this;
    }

    public override int GetHashCode()
    {
        return _message.GetHashCode();
    }
}
