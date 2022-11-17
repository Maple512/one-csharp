namespace OneI.Logable.Properties.Internal;

using System;
using System.IO;

public class ObjectPropertyValue : PropertyValue
{
    private readonly IPropertyValue[] _properties;

    public override void Render(TextWriter output, string? format = null, IFormatProvider? formatProvider = null)
    {
        throw new NotImplementedException();
    }
}
