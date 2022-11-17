namespace OneI.Logable.Properties.Internal;

using System;
using System.Globalization;
using System.IO;
using OneI.Logable.Properties;

public class StructPropertyValue<TValue> : PropertyValue<TValue>
    where TValue : struct
{
    public StructPropertyValue(TValue value) : base(value)
    {
    }

    public override void Render(TextWriter output, string? format = null, IFormatProvider? formatProvider = null)
    {
        if(formatProvider is ICustomFormatter customFormatter)
        {
            output.Write(customFormatter.Format(format, Value, formatProvider));
            return;
        }

        switch(Value)
        {
            case IFormattable formattable:
                output.Write(formattable.ToString(format, formatProvider ?? CultureInfo.InvariantCulture));
                break;
            default:
                output.Write(Value.ToString());
                break;
        }
    }
}
