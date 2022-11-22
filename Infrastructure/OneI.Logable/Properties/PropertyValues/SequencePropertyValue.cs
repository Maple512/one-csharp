namespace OneI.Logable.Properties.PropertyValues;

using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 队列
/// </summary>
public class SequencePropertyValue : PropertyValue
{
    public SequencePropertyValue(IReadOnlyList<PropertyValue> values)
    {
        Values = values;
    }

    public IReadOnlyList<PropertyValue> Values { get; }

    public override void Render(TextWriter output, string? format = null, IFormatProvider? formatProvider = null)
    {
        output.Write('[');

        var first = true;
        foreach(var value in Values)
        {
            if(first)
            {
                first = false;
            }
            else
            {
                output.Write(", ");
            }

            value.Render(output, format, formatProvider);
        }

        output.Write(']');
    }
}
