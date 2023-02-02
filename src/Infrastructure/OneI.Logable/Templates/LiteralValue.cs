namespace OneI.Logable.Templates;

public readonly struct LiteralValue<T> : ITemplatePropertyValue
{
    public LiteralValue(T value) => Value = value;

    public T? Value { get; }

    public void Render(TextWriter writer, PropertyType type, string? format, IFormatProvider? formatProvider)
    {
        //TemplateRenderHelper.LiteralRender(writer, Value, type, format, formatProvider);
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var writer = new StringWriter(formatProvider);

        Render(writer, PropertyType.None, format, formatProvider);

        return writer.ToString();
    }

    public override bool Equals(object? obj) => obj is LiteralValue<T> sv && Equals(sv.Value, Value);

    public override int GetHashCode() => Value is null ? 0 : HashCode.Combine(Value);
}
