namespace OneI.Logable.Properties;

using System;
using System.IO;

public abstract class PropertyValue
{
    public abstract void Render(TextWriter output, string? format = null, IFormatProvider? formatProvider = null);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var output = new StringWriter();

        Render(output, format, formatProvider);

        return output.ToString();
    }

    public override string ToString() => ToString(null, null);

    public override int GetHashCode() => 0;
}
