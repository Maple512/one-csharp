namespace OneI.Logable.Properties.Internal;

using System;
using System.IO;

public class StringPropertyValue : PropertyValue<string>
{
    public StringPropertyValue(string value) : base(value)
    {
    }

    public override void Render(TextWriter output, string? format = null, IFormatProvider? formatProvider = null)
    {
        output.Write(Value);
    }
}
