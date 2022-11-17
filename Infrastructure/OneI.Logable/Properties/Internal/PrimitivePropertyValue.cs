namespace OneI.Logable.Properties.Internal;

using System;
using System.IO;
using OneI.Logable.Properties;

public class PrimitivePropertyValue<TValue> : PropertyValue<TValue>
    where TValue : struct
{
    public PrimitivePropertyValue(TValue value) : base(value)
    {
    }

    public override void Render(TextWriter output, string? format = null, IFormatProvider? formatProvider = null)
    {
        output.Write(Value.ToString());
    }
}
