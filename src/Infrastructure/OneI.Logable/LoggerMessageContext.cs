namespace OneI.Logable;

using OneI.Logable.Formatters;
using OneI.Logable.Templatizations;
using OneI.Logable.Templatizations.Tokenizations;
using static OneI.Logable.LoggerConstants;

public class LoggerMessageContext
{
    private readonly List<ITemplateToken> _messageTokens;
    private readonly PropertyCollection _properties = new(20);

    public LoggerMessageContext(
        LogLevel level,
        List<ITemplateToken> tokens,
        Exception? exception,
        string? filePath,
        string? memberName,
        int? lineNumber)
    {
        Timestamp = DateTimeOffset.Now;
        Level = level;
        _messageTokens = tokens;
        Exception = exception;
        File = filePath;
        Member = memberName;
        Line = lineNumber;

        if(filePath.NotNullOrWhiteSpace())
        {
            FileName = Path.GetFileName(filePath);
            FileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            FileExtension = Path.GetExtension(filePath);
        }
    }

    public DateTimeOffset Timestamp { get; }

    public LogLevel Level { get; }

    public Exception? Exception { get; }

    public string? FileName { get; }

    public string? FileNameWithoutExtension { get; }

    public string? FileExtension { get; }

    public string? File { get; }

    public string? Member { get; }

    public int? Line { get; }

    /// <inheritdoc cref="PropertyCollection.Add(string, ITemplatePropertyValue)"/>
    public LoggerMessageContext AddProperty<T>(string name, T value, IPropertyValueFormatter<T>? formatter = null)
    {
        Check.NotNull(name);

        _properties.Add(name, PropertyValue.CreateLiteral(value, formatter));

        return this;
    }

    /// <inheritdoc cref="PropertyCollection.AddOrUpdate(string, ITemplatePropertyValue)"/>
    public LoggerMessageContext AddOrUpdateProperty<T>(string name, T value, IPropertyValueFormatter<T>? formatter = null)
    {
        Check.NotNull(name);

        _properties.AddOrUpdate(name, PropertyValue.CreateLiteral(value, formatter));

        return this;
    }

    /// <inheritdoc cref="PropertyCollection.Add(int, ITemplatePropertyValue)"/>
    public LoggerMessageContext AddProperty<T>(int index, T value, IPropertyValueFormatter<T>? formatter = null)
    {
        _properties.Add(index, PropertyValue.CreateLiteral(value, formatter));

        return this;
    }

    /// <inheritdoc cref="PropertyCollection.AddOrUpdate(int, ITemplatePropertyValue)"/>
    public LoggerMessageContext AddOrUpdateProperty<T>(int index, T value, IPropertyValueFormatter<T>? formatter = null)
    {
        _properties.AddOrUpdate(index, PropertyValue.CreateLiteral(value, formatter));

        return this;
    }

    internal PropertyCollection GetProperties()
    {
        var properties = new PropertyCollection(_properties.Count + 9);
        properties.Add(_properties);

        properties.Add(PropertyNames.Timestamp, PropertyValue.CreateLiteral(Timestamp));
        properties.Add(PropertyNames.Level, PropertyValue.CreateLiteral(Level, LevelFormatter.Instance));
        properties.Add(PropertyNames.Exception, PropertyValue.CreateLiteral(Exception));
        properties.Add(PropertyNames.File, PropertyValue.CreateLiteral(File));
        properties.Add(PropertyNames.FileName, PropertyValue.CreateLiteral(FileName));
        properties.Add(PropertyNames.FileNameWithoutExtension, PropertyValue.CreateLiteral(FileNameWithoutExtension));
        properties.Add(PropertyNames.FileExtension, PropertyValue.CreateLiteral(FileExtension));
        properties.Add(PropertyNames.Member, PropertyValue.CreateLiteral(Member));
        properties.Add(PropertyNames.Line, PropertyValue.CreateLiteral(Line));
        properties.Add(PropertyNames.NewLine, PropertyValue.CreateLiteral(Environment.NewLine));
        properties.Add(PropertyNames.Message, PropertyValue.CreateLiteral(string.Empty,
            new MessageFormatter(_messageTokens, _properties)));

        return properties;
    }

    public LoggerMessageContext AppendText(string text)
    {
        _messageTokens.Add(new TextToken(_messageTokens.Count, text));

        return this;
    }
}
