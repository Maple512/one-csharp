namespace OneI.Logable.Formatting;

using System;
using System.Collections.Generic;
using OneI.Textable;
using OneI.Textable.Templating.Properties;

public class MessageFormatter : IFormatter<string>
{
    private readonly TextTemplate _template;

    public MessageFormatter(TextTemplate messageTemplate, Dictionary<string, PropertyValue> properties)
    {
        _template = messageTemplate
            .WithProperties(properties);
    }

    public void Format(string? value, TextWriter writer, string? format = null, IFormatProvider? formatProvider = null)
    {
        _template.Render(writer, formatProvider);
    }
}
