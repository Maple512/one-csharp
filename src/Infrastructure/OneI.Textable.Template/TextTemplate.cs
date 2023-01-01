namespace OneI.Textable;

using OneI.Textable.Templating;
using OneI.Textable.Templating.Properties;

/// <summary>
/// 文本模板
/// </summary>
public class TextTemplate
{
    private readonly Token[] _tokens;
    private readonly PropertyToken[] _propertyTokens;
    private readonly Dictionary<string, PropertyValue> _properties;

    private TextTemplate(string text, IEnumerable<Token> tokens)
    {
        Text = text;
        _tokens = tokens.ToArray();
        _propertyTokens = tokens.OfType<PropertyToken>().ToArray();
        _properties = new(StringComparer.InvariantCulture);
    }

    /// <summary>
    /// 根据指定的文本，创建一个文本模板
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static TextTemplate Create(string text)
    {
        var tokens = TemplateParser.Parse(text);

        return new(text, tokens);
    }

    public TextTemplate AddProperty<T>(string name, T value, IFormatter<T>? formatter = null)
    {
        _properties.TryAdd(name, PropertyValue.CreateLiteral(value, formatter));

        return this;
    }

    /// <summary>
    /// 清空已存在的所有Property，并设置为给定的<paramref name="properties"/>
    /// </summary>
    /// <param name="properties"></param>
    /// <returns></returns>
    public TextTemplate WithProperties(IDictionary<string, PropertyValue> properties)
    {
        _properties.Clear();

        foreach(var item in properties)
        {
            _properties[item.Key] = item.Value;
        }

        return this;
    }

    public void Render(TextWriter writer, IFormatProvider? formatProvider = null)
    {
        TemplateRenderer.Render(this, writer, formatProvider);
    }

    public string Text { get; }

    public IReadOnlyList<Token> Tokens => _tokens;

    public IReadOnlyList<PropertyToken> PropertyTokens => _propertyTokens;

    public IReadOnlyDictionary<string, PropertyValue> Properties => _properties;

    public string ToString(IFormatProvider? formatProvider = null)
    {
        var writer = new StringWriter();

        Render(writer, formatProvider);

        return writer.ToString();
    }

    public override string ToString() => Text;
}
