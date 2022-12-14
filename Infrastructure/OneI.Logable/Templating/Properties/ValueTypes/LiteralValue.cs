namespace OneI.Logable.Templating.Properties.ValueTypes;

using OneI.Logable.Templating.Rendering;

/// <summary>
/// The literal value.
/// </summary>
/// <typeparam name="T"></typeparam>
public class LiteralValue<T> : PropertyValue
{
    public LiteralValue(T? value, ILoggerSerializable<T>? serializer = null)
    {
        Value = value;
        Serializer = serializer;
    }

    public T? Value { get; }

    public ILoggerSerializable<T>? Serializer { get; }

    public override void Render(TextWriter writer, string? format = null, IFormatProvider? formatProvider = null)
    {
        TextTemplateRenderer.DefaultRender(Value, Serializer, writer, format, formatProvider);
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
