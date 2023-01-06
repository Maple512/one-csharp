namespace OneI.Logable;

using OneI.Diagnostics;
using OneI.Logable.Formatting;
using OneI.Textable;
using OneI.Textable.Templating;
using OneI.Textable.Templating.Properties;
using static OneI.Logable.LoggerConstants;

[Serializable]
public struct LoggerContext
{
    private readonly Dictionary<string, PropertyValue> _properties;

    public LoggerContext(
        LogLevel level,
        TextTemplate messageTemplate,
        Exception? exception = null,
        Dictionary<string, PropertyValue>? properties = null,
        string? filePath = null,
        string? memberName = null,
        int? lineNumber = null)
    {
        Timestamp = Clock.Now;
        Level = level;
        MessageTemplate = Check.NotNull(messageTemplate);
        Exception = exception;
        _properties = properties ?? new();

        MemberName = memberName;
        FilePath = filePath;
        LineNumber = lineNumber;
    }

    /// <summary>
    /// Gets the timestamp.
    /// </summary>
    public DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Gets the level.
    /// </summary>
    public LogLevel Level { get; }

    /// <summary>
    /// Gets the message template.
    /// </summary>
    public TextTemplate MessageTemplate { get; }

    /// <summary>
    /// Gets the exception.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Gets the file path.
    /// </summary>
    public string? FilePath { get; }

    /// <summary>
    /// Gets the member name.
    /// </summary>
    public string? MemberName { get; }

    /// <summary>
    /// Gets the line number.
    /// </summary>
    public int? LineNumber { get; }

    /// <summary>
    /// Gets the properties.
    /// </summary>
    public IReadOnlyDictionary<string, PropertyValue> Properties => _properties;

    /// <summary>
    /// Adds the or update property.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <returns>A LoggerContext.</returns>
    public LoggerContext AddOrUpdateProperty(string name, PropertyValue value)
    {
        _properties[name] = value;

        return this;
    }

    /// <summary>
    /// Adds the property if absent.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <returns>A LoggerContext.</returns>
    public LoggerContext AddPropertyIfAbsent<T>(string name, T value)
    {
        return AddPropertyIfAbsent(name, PropertyValue.CreateLiteral(value));
    }

    /// <summary>
    /// Adds the property if absent.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <returns>A LoggerContext.</returns>
    public LoggerContext AddPropertyIfAbsent(string name, PropertyValue value)
    {
        if(_properties.ContainsKey(name) == false)
        {
            _properties.Add(name, value);
        }

        return this;
    }

    /// <summary>
    /// Removes the property.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>A LoggerContext.</returns>
    public LoggerContext RemoveProperty(string name)
    {
        _properties.Remove(name);

        return this;
    }

    //public LoggerContext Append(scoped in ReadOnlySpan<char> text)
    //{
    //    MessageTemplate.Append(text);

    //    return this;
    //}

    /// <summary>
    /// Copies the.
    /// </summary>
    /// <returns>A LoggerContext.</returns>
    internal LoggerContext Copy()
    {
        var properties = new Dictionary<string, PropertyValue>(_properties.Count);

        foreach(var property in _properties)
        {
            properties.Add(property.Key, property.Value);
        }

        return new(Level, MessageTemplate, Exception, properties, FilePath, MemberName, LineNumber);
    }

    internal PropertyCollection GetAllProperties()
    {
        var properties = _properties;

        properties.Add(nameof(Timestamp), PropertyValue.CreateLiteral(Timestamp));
        properties.Add(nameof(Level), PropertyValue.CreateLiteral(Level, LevelFormatter.Instance));
        properties.Add(nameof(Exception), PropertyValue.CreateLiteral(Exception));
        properties.Add(nameof(FilePath), PropertyValue.CreateLiteral(FilePath));
        properties.Add(nameof(MemberName), PropertyValue.CreateLiteral(MemberName));
        properties.Add(nameof(LineNumber), PropertyValue.CreateLiteral(LineNumber));
        properties.Add(PropertyNames.NewLine, PropertyValue.CreateLiteral(Environment.NewLine));
        properties.Add(PropertyNames.Message, PropertyValue.CreateLiteral(string.Empty, new MessageFormatter(MessageTemplate, _properties)));

        return new PropertyCollection(properties);
    }
}
