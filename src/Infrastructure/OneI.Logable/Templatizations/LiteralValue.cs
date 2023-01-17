namespace OneI.Logable.Templatizations;

using OneI.Logable.Templatizations.Tokenizations;

public class LiteralValue<T> : PropertyValue
{
    public LiteralValue(T? value, IPropertyValueFormatter<T>? formatter = null)
    {
        Value = value;
        Formatter = formatter;
    }

    public T? Value { get; }

    public IPropertyValueFormatter<T>? Formatter { get; }

    public override void Render(TextWriter writer, in PropertyTokenType type, in string? format, IFormatProvider? formatProvider)
    {
        TemplateRenderHelper.Render(Value, writer, type, format, Formatter, formatProvider);
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
