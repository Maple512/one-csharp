namespace OneI.Logable.Properties.PropertyValues;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// 键-值
/// </summary>
public class StructurePropertyValue : PropertyValue
{
    public StructurePropertyValue(IEnumerable<Property> properties, string? type = default)
    {
        Type = type;
        Properties = properties.ToArray();
    }

    public string? Type { get; }

    public Property[] Properties { get; }

    public override void Render(TextWriter output, string? format = null, IFormatProvider? formatProvider = null)
    {
        if(Type != null)
        {
            output.Write(Type);
            output.Write(' ');
        }

        output.Write('{');

        var first = true;
        foreach(var property in Properties)
        {
            if(first)
            {
                first = false;
            }
            else
            {
                output.Write(", ");
            }

            output.Write(property.Name);
            output.Write(": ");
            output.Write(property.Value);
        }

        output.Write('}');
    }
}
