namespace OneI.Logable.Formatting;

using System;
using OneI.Textable;
using OneI.Textable.Templating.Properties;

public class MessageFormatter : IFormatter<string>
{
    private readonly TemplateContext _templateContext;
    private readonly Dictionary<string, PropertyValue> _properties;

    public MessageFormatter(TemplateContext templateContext, Dictionary<string, PropertyValue> properties)
    {
        _properties = properties;
        _templateContext = templateContext;
    }

    public void Format(string? value, TextWriter writer, string? format = null, IFormatProvider? formatProvider = null)
    {
        var options = new TemplateOptions(new(_properties), formatProvider);

        TemplateRenderer.Render(_templateContext, writer, options);
    }
}
