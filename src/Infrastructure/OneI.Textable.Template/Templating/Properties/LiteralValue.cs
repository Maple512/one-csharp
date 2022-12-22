namespace OneI.Textable.Templating.Properties;

using OneI.Textable;
using OneI.Textable.Rendering;
/// <summary>
/// The literal value.
/// </summary>

public class LiteralValue<T> : PropertyValue
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiteralValue"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="formatter">The formatter.</param>
    public LiteralValue(T? value, IFormatter<T>? formatter = null)
    {
        Value = value;
        Formatter = formatter;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Gets the formatter.
    /// </summary>
    public IFormatter<T>? Formatter { get; }

    /// <summary>
    /// Renders the.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="format">The format.</param>
    /// <param name="formatProvider">The format provider.</param>
    public override void Render(TextWriter writer, string? format = null, IFormatProvider? formatProvider = null)
    {
        RenderHelper.Render(Value, Formatter, writer, format, formatProvider);
    }

    /// <summary>
    /// Equals the.
    /// </summary>
    /// <param name="obj">The obj.</param>
    /// <returns>A bool.</returns>
    public override bool Equals(object? obj)
    {
        return obj is LiteralValue<T> sv && Equals(sv.Value, Value);
    }

    /// <summary>
    /// Gets the hash code.
    /// </summary>
    /// <returns>An int.</returns>
    public override int GetHashCode()
    {
        return Value is null ? 0 : Value.GetHashCode();
    }
}
