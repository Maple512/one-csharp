namespace OneI.Logable.Properties.PropertyValues;

using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 字典
/// </summary>
public class DicationaryPropertyValue : PropertyValue
{
    public IReadOnlyList<KeyValuePair<ScalarPropertyValue, PropertyValue>> Values { get; }

    public DicationaryPropertyValue(IReadOnlyList<KeyValuePair<ScalarPropertyValue, PropertyValue>> values)
    {
        Values = values;
    }

    public override void Render(TextWriter output, string? format = null, IFormatProvider? formatProvider = null)
    {
        output.Write('[');

        var first = true;
        var index = 0;
        foreach(var item in Values)
        {
            if(first)
            {
                output.Write('(');
                first = false;
            }
            else
            {
                output.Write(", (");
            }

            item.Key.Render(output, format, formatProvider);
            output.Write(':');
            item.Value.Render(output, format, formatProvider);

            output.Write(')');

            index++;
        }

        output.Write(']');
    }
}
