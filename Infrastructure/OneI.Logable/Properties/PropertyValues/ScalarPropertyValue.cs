namespace OneI.Logable.Properties.PropertyValues;

using System;
using System.Globalization;
using System.IO;

/// <summary>
/// 标量
/// </summary>
public class ScalarPropertyValue : PropertyValue
{
    public object? Value { get; }

    public ScalarPropertyValue(object? value)
    {
        Value = value;
    }

    public override void Render(TextWriter output, string? format = null, IFormatProvider? formatProvider = null)
    {
        Render(Value, output, format, formatProvider);
    }

    internal static void Render(object? value, TextWriter output, string? format = null, IFormatProvider? formatProvider = null)
    {
        if(value == null)
        {
            output.Write("null");
            return;
        }

        if(value is string s)
        {
            if(format != "l")
            {
                output.Write("\"");
                output.Write(s.Replace("\"", "\\\""));
                output.Write("\"");
            }
            else
            {
                output.Write(s);
            }

            return;
        }

        var custom = (ICustomFormatter?)formatProvider?.GetFormat(typeof(ICustomFormatter));
        if(custom != null)
        {
            output.Write(custom.Format(format, value, formatProvider));
            return;
        }

        if(value is IFormattable formattable)
        {
            output.Write(formattable.ToString(format, formatProvider ?? CultureInfo.InvariantCulture));
        }
        else
        {
            output.Write(value.ToString());
        }
    }

    public override bool Equals(object? obj)
    {
        return obj is ScalarPropertyValue sp && sp.Value == Value;
    }

    public override int GetHashCode() => Value == null ? 0 : Value.GetHashCode();
}
