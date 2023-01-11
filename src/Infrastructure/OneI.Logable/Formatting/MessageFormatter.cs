namespace OneI.Logable.Formatting;

using System;
using System.Collections.Generic;
using OneI.Logable;
using OneI.Logable.Templating.Properties;

public class MessageFormatter : IFormatter<string>
{
    private readonly ReadOnlyMemory<char> _template;
    private readonly Dictionary<string, PropertyValue> _properties;

    public MessageFormatter(
        ReadOnlyMemory<char> template,
        Dictionary<string, PropertyValue> properties)
    {
        _template = template;
        _properties = properties;
    }

    public void Format(string? value, TextWriter writer, string? format = null, IFormatProvider? formatProvider = null)
    {
        var template = TextTemplate.Create(_template);

        template.WithProperties(_properties);

        template.Render(writer, formatProvider);
    }
}
