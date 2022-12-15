namespace OneI.Logable;

using OneI.Logable.Templating;
using OneI.Logable.Templating.Properties;

[Serializable]
public class LoggerContext
{
    private readonly Dictionary<string, PropertyValue> _properties;

    public LoggerContext(
        LogLevel level,
        TextTemplate textTemplate,
        Exception? exception = null,
        List<Property>? properties = null)
    {
        Timestamp = DateTimeOffset.Now;
        Level = level;
        TextTemplate = textTemplate;
        Exception = exception;
        _properties = properties?.ToDictionary(k => k.Name, v => v.Value)
            ?? new();
    }

    private LoggerContext(
        LogLevel level,
        TextTemplate textTemplate,
        Exception? exception = null,
        Dictionary<string, PropertyValue>? properties = null)
    {
        Timestamp = DateTimeOffset.Now;
        Level = level;
        TextTemplate = textTemplate;
        Exception = exception;
        _properties = properties ?? new();
    }

    public DateTimeOffset Timestamp { get; }

    public LogLevel Level { get; }

    public TextTemplate TextTemplate { get; }

    public Exception? Exception { get; }

    //public string? MemberName { get; }

    //public string? FilePath { get; }

    //public int? LineNumber { get; }

    public IReadOnlyDictionary<string, PropertyValue> Properties => _properties;

    public LoggerContext AddOrUpdateProperty(Property property)
    {
        _properties[property.Name] = property.Value;

        return this;
    }

    public LoggerContext AddPropertyIfAbsent(Property property)
    {
        if(_properties.ContainsKey(property.Name) == false)
        {
            _properties.Add(property.Name, property.Value);
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

        return new(Level, TextTemplate, Exception, properties);
    }
}
