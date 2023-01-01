namespace OneI.Textable.Templating.Properties;

using OneI.Textable.Rendering;

public class LiteralValue<T> : PropertyValue
{
    public LiteralValue(T? value, IFormatter<T>? formatter = null)
    {
        Value = value;
        Formatter = formatter;
    }

    public T? Value { get; }

    public IFormatter<T>? Formatter { get; }

    public override void Render(TextWriter writer, string? format = null, IFormatProvider? formatProvider = null)
    {
        RenderHelper.Render(Value, Formatter, writer, format, formatProvider);
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
