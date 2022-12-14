namespace OneI.Logable;

using OneI.Logable.Templating;
using OneI.Logable.Templating.Properties;

/// <summary>
/// The logger context.
/// </summary>
[Serializable]
public class LoggerContext
{
    /// <summary>
    /// The properties.
    /// </summary>
    private readonly Dictionary<string, PropertyValue> _properties;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerContext"/> class.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <param name="template">The template.</param>
    /// <param name="exception">The exception.</param>
    /// <param name="properties">The properties.</param>
    public LoggerContext(
        LogLevel level,
        TextTemplate template,
        Exception? exception = null,
        List<Property>? properties = null)
    {
        Timestamp = DateTimeOffset.Now;
        Level = level;
        MessageTemplate = template;
        Exception = exception;
        _properties = properties?.ToDictionary(k => k.Name, v => v.Value)
            ?? new();
    }

    /// <summary>
    /// Gets the timestamp.
    /// </summary>
    /// <value>A DateTimeOffset.</value>
    public DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Gets the level.
    /// </summary>
    /// <value>A LogLevel.</value>
    public LogLevel Level { get; }

    /// <summary>
    /// Gets the message template.
    /// </summary>
    /// <value>A TextTemplate.</value>
    public TextTemplate MessageTemplate { get; }

    /// <summary>
    /// Gets the exception.
    /// </summary>
    /// <value>An Exception? .</value>
    public Exception? Exception { get; }

    //public string? MemberName { get; }

    //public string? FilePath { get; }

    //public int? LineNumber { get; }

    /// <summary>
    /// Gets the properties.
    /// </summary>
    /// <value>A dictionary with a key of type string and a value of type propertyvalues.</value>
    public IReadOnlyDictionary<string, PropertyValue> Properties => _properties;

    /// <summary>
    /// Add or update property.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns>A LoggerContext.</returns>
    public LoggerContext AddOrUpdateProperty(Property property)
    {
        _properties[property.Name] = property.Value;

        return this;
    }

    /// <summary>
    /// Add property if absent.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns>A LoggerContext.</returns>
    public LoggerContext AddPropertyIfAbsent(Property property)
    {
        if(_properties.ContainsKey(property.Name) == false)
        {
            _properties.Add(property.Name, property.Value);
        }

        return this;
    }

    /// <summary>
    /// Remove property.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>A LoggerContext.</returns>
    public LoggerContext RemoveProperty(string name)
    {
        _properties.Remove(name);

        return this;
    }
}
