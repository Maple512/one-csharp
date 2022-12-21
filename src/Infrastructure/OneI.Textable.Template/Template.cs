namespace OneI.Textable;

using OneI.Textable.Templating.Properties;

public class Template
{
    private readonly string _text;
    private readonly IFormatProvider? _formatProvider;
    private readonly Dictionary<string, PropertyValue> _properties;

    private TemplateContext? _context;

    private Template(string text, IFormatProvider? formatProvider = null)
    {
        _context = null;
        _text = text;
        _properties = new();
        _formatProvider = formatProvider;
    }

    public static Template Create(string text, IFormatProvider? _formatProvider = null)
    {
        return new(text, _formatProvider);
    }

    public Template With<T>(string name, T value)
    {
        _properties.TryAdd(name, PropertyValue.CreateLiteral(value));

        return this;
    }

    public Template With(string name, PropertyValue value)
    {
        _properties.TryAdd(name, value);

        return this;
    }

    public TemplateContext Parse()
    {
        if(_context != null)
        {
            return _context;
        }

        _context = TemplateParser.Parse(_text);

        return _context;
    }

    public void Render(TextWriter writer)
    {
        TemplateRenderer.Render(Parse(), writer, new TemplateOptions(_properties, _formatProvider));
    }

    public override string ToString()
    {
        var writer = new StringWriter();

        Render(writer);

        return writer.ToString();
    }
}
