namespace OneI.Logable;

using OneI.Diagnostics;
using OneI.Logable.Formatting;
using OneI.Textable;
using OneI.Textable.Templating;
using OneI.Textable.Templating.Properties;
using static OneI.Logable.LoggerConstants;

[Serializable]
public class LoggerContext
{
    private readonly Dictionary<string, PropertyValue> _properties;

    public LoggerContext(
        LogLevel level,
        TemplateContext messageTemplate,
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

    public DateTimeOffset Timestamp { get; }

    public LogLevel Level { get; }

    public TemplateContext MessageTemplate { get; }

    public Exception? Exception { get; }

    public string? FilePath { get; }

    public string? MemberName { get; }

    public int? LineNumber { get; }

    public IReadOnlyDictionary<string, PropertyValue> Properties => _properties;

    public LoggerContext AddOrUpdateProperty(string name, PropertyValue value)
    {
        _properties[name] = value;

        return this;
    }

    public LoggerContext AddPropertyIfAbsent<T>(string name, T value)
    {
        return AddPropertyIfAbsent(name, PropertyValue.CreateLiteral(value));
    }

    public LoggerContext AddPropertyIfAbsent(string name, PropertyValue value)
    {
        if(_properties.ContainsKey(name) == false)
        {
            _properties.Add(name, value);
        }

        return this;
    }

    public LoggerContext RemoveProperty(string name)
    {
        _properties.Remove(name);

        return this;
    }

    internal LoggerContext Copy()
    {
        var properties = new Dictionary<string, PropertyValue>(_properties.Count);

        foreach(var property in _properties)
        {
            properties.Add(property.Key, property.Value);
        }

        return new(Level, MessageTemplate, Exception, properties, FilePath, MemberName, LineNumber);
    }

    public virtual PropertyCollection GetAllProperties()
    {
        var properties = _properties;

        properties.Add(nameof(Timestamp), PropertyValue.CreateLiteral(Timestamp));
        properties.Add(nameof(Level), PropertyValue.CreateLiteral(Level, new LevelFormatter()));
        properties.Add(nameof(Exception), PropertyValue.CreateLiteral(Exception));
        properties.Add(nameof(FilePath), PropertyValue.CreateLiteral(FilePath));
        properties.Add(nameof(MemberName), PropertyValue.CreateLiteral(MemberName));
        properties.Add(nameof(LineNumber), PropertyValue.CreateLiteral(LineNumber));
        properties.Add(PropertyNames.NewLine, PropertyValue.CreateLiteral(null, new NewLineFormatter()));
        properties.Add(PropertyNames.Message, PropertyValue.CreateLiteral(PropertyNames.Message, new MessageFormatter(MessageTemplate, _properties)));

        return new PropertyCollection(properties);
    }
}
