namespace OneI.Logable;

using DotNext;
using Formatters;
using Templatizations;
using static LoggerConstants;

public readonly struct LoggerMessageContext
{
    internal LoggerMessageContext(
        LogLevel level,
        ReadOnlyMemory<char> message,
        in Exception? exception,
        ValueDictionary<string, ITemplatePropertyValue> properties,
        ReadOnlyMemory<char> filePath,
        ReadOnlyMemory<char> memberName,
        int line)
    {
        Timestamp = DateTimeOffset.Now;
        Level = level;
        Message = message;
        Properties = properties;
        Exception = exception;
        File = filePath;
        Member = memberName;
        Line = line;

        //if(filePath is not { Length: > 0 })
        //{
        //    return;
        //}
        //scoped var path = File.AsSpan();

        //var index = path.LastIndexOf(Path.DirectorySeparatorChar) + 1;
        //var fileName = path[index..];
        //var i2 = fileName.LastIndexOf('.');

        //FileName = fileName.ToString();
        //FileNameWithoutExtension = fileName[..i2].ToString();
        //FileExtension = fileName[i2..].ToString();
    }

    public DateTimeOffset Timestamp { get; }

    public LogLevel Level { get; }

    public ReadOnlyMemory<char> Message { get; }

    public Exception? Exception { get; }

    public ReadOnlyMemory<char> FileName { get; }

    public ReadOnlyMemory<char> FileNameWithoutExtension { get; }

    public ReadOnlyMemory<char> FileExtension { get; }

    public ReadOnlyMemory<char> File { get; }

    public ReadOnlyMemory<char> Member { get; }

    public ValueDictionary<string, ITemplatePropertyValue> Properties
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    public int Line { get; }

    /// <inheritdoc cref="PropertyCollection.Add(string, ITemplatePropertyValue)"/>
    public LoggerMessageContext AddProperty<T>(string name, T value, IPropertyValueFormatter<T>? formatter = null)
    {
        Check.NotNull(name);

        Properties.Add(name, PropertyValue.CreateLiteral(value, formatter));

        return this;
    }

    /// <inheritdoc cref="PropertyCollection.AddOrUpdate(string, ITemplatePropertyValue)"/>
    public LoggerMessageContext AddOrUpdateProperty<T>(string name, T value, IPropertyValueFormatter<T>? formatter = null)
    {
        Check.NotNull(name);

        Properties.Add(name, PropertyValue.CreateLiteral(value, formatter));

        return this;
    }

    [Obsolete]
    internal ValueDictionary<string, ITemplatePropertyValue> GetProperties()
    {
        var properties = new ValueDictionary<string, ITemplatePropertyValue>(Properties, 11);

        properties.Add(PropertyNames.Timestamp, new LiteralValue<DateTimeOffset>(Timestamp));
        properties.Add(PropertyNames.Level, new LiteralValue<LogLevel>(Level, LevelFormatter.Instance));
        properties.Add(PropertyNames.Exception, new LiteralValue<Exception?>(Exception));
        properties.Add(PropertyNames.File, new LiteralValue<ReadOnlyMemory<char>>(File));
        properties.Add(PropertyNames.FileName, new LiteralValue<ReadOnlyMemory<char>>(FileName));
        properties.Add(PropertyNames.FileNameWithoutExtension, new LiteralValue<ReadOnlyMemory<char>>(FileNameWithoutExtension));
        properties.Add(PropertyNames.FileExtension, new LiteralValue<ReadOnlyMemory<char>>(FileExtension));
        properties.Add(PropertyNames.Member, new LiteralValue<ReadOnlyMemory<char>>(Member));
        properties.Add(PropertyNames.Line, new LiteralValue<int>(Line));
        properties.Add(PropertyNames.NewLine, new LiteralValue<string>(Environment.NewLine));
        properties.Add(PropertyNames.Message, new LiteralValue<ReadOnlyMemory<char>>(Message));

        return properties;
    }

    public LoggerMessageContext AppendText(scoped ReadOnlySpan<char> text)
    {
        if(!text.IsEmpty)
        {
            Message.Span.Concat(text);
        }

        return this;
    }
}
