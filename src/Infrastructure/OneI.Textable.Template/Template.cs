namespace OneI.Textable;

using OneI.Textable.Templating.Properties;
/// <summary>
/// The template.
/// </summary>

public class Template
{
    private readonly string _text;
    private readonly IFormatProvider? _formatProvider;
    private readonly Dictionary<string, PropertyValue> _properties;

    private TemplateContext? _context;

    /// <summary>
    /// Prevents a default instance of the <see cref="Template"/> class from being created.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="formatProvider">The format provider.</param>
    private Template(string text, IFormatProvider? formatProvider = null)
    {
        _context = null;
        _text = text;
        _properties = new();
        _formatProvider = formatProvider;
    }

    /// <summary>
    /// Creates the.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="_formatProvider">The _format provider.</param>
    /// <returns>A Template.</returns>
    public static Template Create(string text, IFormatProvider? _formatProvider = null)
    {
        return new(text, _formatProvider);
    }

    /// <summary>
    /// Withs the.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <returns>A Template.</returns>
    public Template With<T>(string name, T value)
    {
        _properties.TryAdd(name, PropertyValue.CreateLiteral(value));

        return this;
    }

    /// <summary>
    /// Withs the.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <returns>A Template.</returns>
    public Template With(string name, PropertyValue value)
    {
        _properties.TryAdd(name, value);

        return this;
    }

    /// <summary>
    /// Parses the.
    /// </summary>
    /// <returns>A TemplateContext.</returns>
    public TemplateContext Parse()
    {
        if(_context != null)
        {
            return _context;
        }

        _context = TemplateParser.Parse(_text);

        return _context;
    }

    /// <summary>
    /// Renders the.
    /// </summary>
    /// <param name="writer">The writer.</param>
    public void Render(TextWriter writer)
    {
        TemplateRenderer.Render(Parse(), writer, new TemplateOptions(_properties, _formatProvider));
    }

    /// <summary>
    /// Tos the string.
    /// </summary>
    /// <returns>A string.</returns>
    public override string ToString()
    {
        var writer = new StringWriter();

        Render(writer);

        return writer.ToString();
    }
}
