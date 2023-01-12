namespace OneI.Logable;

using System;
using DotNext;
using DotNext.Buffers;
using OneI.Logable.Templates;
using OneI.Logable.Templating;
using OneI.Logable.Templating.Properties;

/// <summary>
/// 文本模板
/// </summary>
public class TextTemplate
{
    private readonly ReadOnlyMemory<char> _buffer;
    private IEnumerable<Token>? _tokens;
    private Dictionary<string, PropertyValue> _properties;

    private TextTemplate(in ReadOnlyMemory<char> buffer)
    {
        _buffer = buffer;
        _properties = new();
    }

    /// <summary>
    /// 根据指定的文本，创建一个文本模板
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TextTemplate Create(in string text)
    {
        if(text is null)
        {
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
        }

        return new(text.AsMemory());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TextTemplate Create(in ReadOnlyMemory<char> memory)
    {
        return new(memory);
    }

    public TextTemplate Append(scoped in ReadOnlySpan<char> text)
    {
        _buffer.Span.Concat(text);

        return this;
    }

    public TextTemplate AddProperty<T>(T value, IFormatter<T>? formatter = null)
    {
        _properties.TryAdd($"property_{_properties.Count}", PropertyValue.CreateLiteral(value, formatter));

        return this;
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
        _properties = new Dictionary<string, PropertyValue>();

        foreach(var item in properties)
        {
            _properties[item.Key] = item.Value;
        }

        return this;
    }

    public void Render(TextWriter writer, IFormatProvider? formatProvider = null)
    {
        var tokens = _tokens ?? TemplateParser.Parse(_buffer.ToString());

        TemplateRenderer.Render(this, writer, tokens, formatProvider);
    }

    public IEnumerable<Token> Tokens
    {
        get
        {
            _tokens ??= TemplateParser.Parse(_buffer.ToString());

            return _tokens;
        }
    }

    public IReadOnlyDictionary<string, PropertyValue> Properties => _properties;

    public string ToString(IFormatProvider? formatProvider = null)
    {
        var writer = new StringWriter();

        Render(writer, formatProvider);

        return writer.ToString();
    }

    public override string ToString()
    {
        return ToString(null);
    }
}
