namespace OneI.Logable.Templatizations;

using Tokenizations;

public readonly struct LiteralValue<T> : ITemplatePropertyValue
{
    public LiteralValue(T value, IPropertyValueFormatter<T>? formatter = null)
    {
        Value = value;
        Formatter = formatter;
    }

    public T? Value { get; }

    public IPropertyValueFormatter<T>? Formatter { get; }

    public void Render(TextWriter writer, PropertyTokenType type, string? format, IFormatProvider? formatProvider)
    {
        TemplateRenderHelper.Render(Value, writer, type, format, Formatter, formatProvider);
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var writer = new StringWriter(formatProvider);

        Render(writer, PropertyTokenType.None, format, formatProvider);

        return writer.ToString();
    }

    public override bool Equals(object? obj)
    {
        return obj is LiteralValue<T> sv && Equals(sv.Value, Value);
    }

    public override int GetHashCode()
    {
        return Value is null ? 0 : Value.GetHashCode();
    }
}
