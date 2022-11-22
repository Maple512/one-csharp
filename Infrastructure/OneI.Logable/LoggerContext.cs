namespace OneI.Logable;

using System;
using System.Collections.Generic;
using OneI.Logable.Parsing;
using OneI.Logable.Properties;

public class LoggerContext
{
    private readonly Dictionary<string, PropertyValue> _properties;

    public LoggerContext(
        LogLevel level,
        Exception? exception,
        IReadOnlyList<Token> tokens,
        IReadOnlyList<Property> properties,
        string? memberName = null,
        string? filePath = null,
        int? lineNumber = null)
        : this(level, null, exception, tokens, new Dictionary<string, PropertyValue>(properties.Count), memberName, filePath, lineNumber)
    {
        foreach(var item in properties)
        {
            _properties[item.Name] = item.Value;
        }
    }

    public LoggerContext(
        LogLevel level,
        string? text,
        Exception? exception,
        IReadOnlyList<Token> tokens,
        Dictionary<string, PropertyValue> properties,
        string? memberName = null,
        string? filePath = null,
        int? lineNumber = null)
    {
        Timestamp = DateTimeOffset.Now;
        Level = level;
        Text = text;
        Exception = exception;
        _properties = properties;
        Tokens = tokens;
        MemberName = memberName;
        FilePath = filePath;
        LineNumber = lineNumber;
    }

    public DateTimeOffset Timestamp { get; }

    public LogLevel Level { get; }

    public string? Text { get; }

    public Exception? Exception { get; }

    public IReadOnlyDictionary<string, PropertyValue> Properties => _properties;

    public IReadOnlyList<Token> Tokens { get; }

    /// <summary>
    /// called method/property name
    /// </summary>
    public string? MemberName { get; init; }

    /// <summary>
    /// called file path
    /// </summary>
    public string? FilePath { get; init; }

    /// <summary>
    /// called file line number
    /// </summary>
    public int? LineNumber { get; init; }

    public LoggerContext AddProperty(in Property property)
    {
        _properties[property.Name] = property.Value;

        return this;
    }

    /// <summary>
    /// 增加一个<see cref="Property"/>，如果<see cref="Properties"/>中不存在
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    public LoggerContext AddPropertyIfAbsent(in Property property)
    {
        if(Properties.ContainsKey(property.Name) == false)
        {
            _properties.Add(property.Name, property.Value);
        }

        return this;
    }

    public LoggerContext RemoveProperty(string propertyName)
    {
        _properties.Remove(propertyName);

        return this;
    }

    public LoggerContext Copy()
    {
        var properties = new Dictionary<string, PropertyValue>(Properties.Count);
        foreach(var item in Properties)
        {
            properties[item.Key] = item.Value;
        }

        return new(
            Level,
            Text,
            Exception,
            Tokens,
            _properties,
            MemberName,
            FilePath,
            LineNumber);
    }
}
