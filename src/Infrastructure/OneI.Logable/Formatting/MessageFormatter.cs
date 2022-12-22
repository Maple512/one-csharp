namespace OneI.Logable.Formatting;

using System;
using OneI.Textable;
using OneI.Textable.Templating.Properties;
/// <summary>
/// The message formatter.
/// </summary>

public class MessageFormatter : IFormatter<string>
{
    private readonly TemplateContext _templateContext;
    private readonly Dictionary<string, PropertyValue> _properties;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageFormatter"/> class.
    /// </summary>
    /// <param name="templateContext">The template context.</param>
    /// <param name="properties">The properties.</param>
    public MessageFormatter(TemplateContext templateContext, Dictionary<string, PropertyValue> properties)
    {
        _properties = properties;
        _templateContext = templateContext;
    }

    /// <summary>
    /// Formats the.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="writer">The writer.</param>
    /// <param name="format">The format.</param>
    /// <param name="formatProvider">The format provider.</param>
    public void Format(string? value, TextWriter writer, string? format = null, IFormatProvider? formatProvider = null)
    {
        var options = new TemplateOptions(new(_properties), formatProvider);

        TemplateRenderer.Render(_templateContext, writer, options);
    }
}
