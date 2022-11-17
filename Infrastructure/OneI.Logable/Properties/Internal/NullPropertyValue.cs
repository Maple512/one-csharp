namespace OneI.Logable.Properties.Internal;

using System;
using System.IO;

public sealed class NullPropertyValue : PropertyValue
{
    internal static IPropertyValue Instance => new NullPropertyValue();

    public override void Render(TextWriter output, string? format = null, IFormatProvider? formatProvider = null)
    {
        output.Write("null");
    }
}
