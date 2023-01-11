namespace OneI.Logable;

using System;
using DotNext;
using OneI.Diagnostics;
using OneI.Logable.Formatting;
using OneI.Logable;
using OneI.Logable.Templating;
using OneI.Logable.Templating.Properties;
using static OneI.Logable.LoggerConstants;

[Serializable]
public readonly struct LoggerContext
{
    private readonly Dictionary<string, PropertyValue> _properties;

    public LoggerContext(
        LogLevel level,
        in ReadOnlyMemory<char> template,
        Exception? exception = null,
        Dictionary<string, PropertyValue>? properties = null,
        string? filePath = null,
        string? memberName = null,
        int? lineNumber = null)
    {
        Timestamp = Clock.Now;
        Level = level;
        MessageTemplate = template.ToString().AsMemory();
        Exception = exception;
        _properties = properties ?? new();

        MemberName = memberName;
        FilePath = filePath;
        LineNumber = lineNumber;
    }

    public DateTimeOffset Timestamp { get; }

    public LogLevel Level { get; }

    public ReadOnlyMemory<char> MessageTemplate { get; }

    public Exception? Exception { get; }

    public string? FileName
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if(FilePath is not null or { Length: 0 })
            {
                return null;
            }

            return Path.GetFileName(FilePath);
        }
    }

    public string? FileNameWithoutExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if(FilePath is not null or { Length: 0 })
            {
                return null;
            }

            return Path.GetFileNameWithoutExtension(FilePath);
        }
    }

    public string? FileExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if(FilePath is not null or { Length: 0 })
            {
                return null;
            }

            return Path.GetExtension(FilePath);
        }
    }

    public string? FilePath { get; }

    public string? MemberName { get; }

    public int? LineNumber { get; }

    public IReadOnlyDictionary<string, PropertyValue> Properties => _properties;

    public LoggerContext AddOrUpdateProperty(string name, PropertyValue value)
    {
        _properties[name] = value;

        return this;
    }

    public LoggerContext AddPropertyIfAbsent<T>(string name, T value, IFormatter<T>? formatter = null)
    {
        return AddPropertyIfAbsent(name, PropertyValue.CreateLiteral(value, formatter));
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

    public LoggerContext AppendText(scoped in ReadOnlySpan<char> text)
    {
        MessageTemplate.Span.Concat(text);

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

    internal PropertyCollection GetAllProperties()
    {
        var properties = _properties;

        properties.Add(nameof(Timestamp), PropertyValue.CreateLiteral(Timestamp));
        properties.Add(nameof(Level), PropertyValue.CreateLiteral(Level, LevelFormatter.Instance));
        properties.Add(nameof(Exception), PropertyValue.CreateLiteral(Exception));
        properties.Add(nameof(FilePath), PropertyValue.CreateLiteral(FilePath));
        properties.Add(nameof(FileName), PropertyValue.CreateLiteral(FileName));
        properties.Add(nameof(FileNameWithoutExtension), PropertyValue.CreateLiteral(FileNameWithoutExtension));
        properties.Add(nameof(FileExtension), PropertyValue.CreateLiteral(FileExtension));
        properties.Add(nameof(MemberName), PropertyValue.CreateLiteral(MemberName));
        properties.Add(nameof(LineNumber), PropertyValue.CreateLiteral(LineNumber));
        properties.Add(PropertyNames.NewLine, PropertyValue.CreateLiteral(Environment.NewLine));
        properties.Add(PropertyNames.Message, PropertyValue.CreateLiteral(string.Empty,
            new MessageFormatter(MessageTemplate, _properties)));

        return new PropertyCollection(properties);
    }
}
