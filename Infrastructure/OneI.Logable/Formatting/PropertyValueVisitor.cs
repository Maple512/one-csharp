namespace OneI.Logable.Properties;

using System;
using System.Globalization;
using System.IO;
using OneI.Logable.Properties.PropertyValues;

public class PropertyValueVisitor : PropertyValueVisitor<TextWriter, bool>
{
    private const string _type = "$type";

    public void Format(PropertyValue value, TextWriter output)
    {
        // Parameter order of ITextFormatter is the reverse of the visitor one.
        // In this class, public methods and methods with Format*() names use the
        // (x, output) parameter naming convention.
        Visit(output, value);
    }

    /// <exception cref="ArgumentNullException">When <paramref name="scalar"/> is <code>null</code></exception>
    protected override bool VisitScalar(TextWriter state, ScalarPropertyValue scalar)
    {
        CheckTools.NotNull(scalar);

        FormatLiteralValue(scalar.Value, state);
        return false;
    }

    protected override bool VisitSequence(TextWriter state, SequencePropertyValue sequence)
    {
        CheckTools.NotNull(sequence);

        state.Write('[');
        var delim = "";
        for(var i = 0; i < sequence.Values.Count; i++)
        {
            state.Write(delim);
            delim = ",";
            Visit(state, sequence.Values[i]);
        }

        state.Write(']');
        return false;
    }

    protected override bool VisitStructure(TextWriter state, StructurePropertyValue structure)
    {
        state.Write('{');

        var delim = "";

        for(var i = 0; i < structure.Properties.Length; i++)
        {
            state.Write(delim);
            delim = ",";
            var prop = structure.Properties[i];
            Escape(prop.Name, state);
            state.Write(':');
            Visit(state, prop.Value);
        }

        if(_type != null && structure.Type != null)
        {
            state.Write(delim);
            Escape(_type, state);
            state.Write(':');
            Escape(structure.Type, state);
        }

        state.Write('}');
        return false;
    }

    protected override bool VisitDicationary(TextWriter state, DicationaryPropertyValue dictionary)
    {
        state.Write('{');
        var delim = "";
        foreach(var element in dictionary.Values)
        {
            state.Write(delim);
            delim = ",";
            Escape((element.Key.Value ?? "null").ToString()!, state);
            state.Write(':');
            Visit(state, element.Value);
        }

        state.Write('}');
        return false;
    }

    /// <summary>
    /// Write a literal as a single JSON value, e.g. as a number or string. Override to
    /// support more value types. Don't write arrays/structures through this method - the
    /// active destructuring policies have already indicated the value should be scalar at
    /// this point.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <param name="output">The output</param>
    protected virtual void FormatLiteralValue(object? value, TextWriter output)
    {
        if(value == null)
        {
            FormatNull(output);
            return;
        }

        // Although the linear switch-on-type has apparently worse algorithmic performance than the O(1)
        // dictionary lookup alternative, in practice, it's much to make a few equality comparisons
        // than the hash/bucket dictionary lookup, and since most data will be string (one comparison),
        // numeric (a handful) or an object (two comparisons) the real-world performance of the code
        // as written is as fast or faster.

        if(value is string str)
        {
            FormatString(str, output);
            return;
        }

        if(value is ValueType)
        {
            switch(value)
            {
                case int or uint or long or ulong or decimal or byte or sbyte or short or ushort:
                    FormatExactNumericValue((IFormattable)value, output);
                    return;
                case double d:
                    FormatDoubleValue(d, output);
                    return;
                case float f:
                    FormatFloatValue(f, output);
                    return;
                case bool b:
                    FormatBooleanValue(b, output);
                    return;
                case char:
                    FormatString(value.ToString()!, output);
                    return;
                case DateTime or DateTimeOffset:
                    FormatDateTimeValue((IFormattable)value, output);
                    return;
                case TimeSpan timeSpan:
                    FormatTimeSpanValue(timeSpan, output);
                    return;
                case DateOnly dateOnly:
                    FormatDateOnlyValue(dateOnly, output);
                    return;
                case TimeOnly timeOnly:
                    FormatTimeOnlyValue(timeOnly, output);
                    return;
            }
        }

        FormatLiteralObjectValue(value, output);
    }

    private static void FormatBooleanValue(bool value, TextWriter output)
    {
        output.Write(value ? "true" : "false");
    }

    private static void FormatFloatValue(float value, TextWriter output)
    {
        if(float.IsNaN(value) || float.IsInfinity(value))
        {
            FormatString(value.ToString(CultureInfo.InvariantCulture), output);
            return;
        }

        output.Write(value.ToString("R", CultureInfo.InvariantCulture));
    }

    private static void FormatDoubleValue(double value, TextWriter output)
    {
        if(double.IsNaN(value) || double.IsInfinity(value))
        {
            FormatString(value.ToString(CultureInfo.InvariantCulture), output);
            return;
        }

        output.Write(value.ToString("R", CultureInfo.InvariantCulture));
    }

    private static void FormatExactNumericValue(IFormattable value, TextWriter output)
    {
        output.Write(value.ToString(null, CultureInfo.InvariantCulture));
    }

    private static void FormatDateTimeValue(IFormattable value, TextWriter output)
    {
        output.Write('\"');
        output.Write(value.ToString("O", CultureInfo.InvariantCulture));
        output.Write('\"');
    }

    private static void FormatTimeSpanValue(TimeSpan value, TextWriter output)
    {
        output.Write('\"');
        output.Write(value.ToString());
        output.Write('\"');
    }

    private static void FormatDateOnlyValue(DateOnly value, TextWriter output)
    {
        output.Write('\"');
        output.Write(value.ToString("yyyy-MM-dd"));
        output.Write('\"');
    }

    private static void FormatTimeOnlyValue(TimeOnly value, TextWriter output)
    {
        output.Write('\"');
        output.Write(value.ToString("O"));
        output.Write('\"');
    }

    private static void FormatLiteralObjectValue(object value, TextWriter output)
    {
        CheckTools.NotNull(value);

        FormatString(value.ToString() ?? "", output);
    }

    private static void FormatString(string value, TextWriter output)
    {
        Escape(value.AsSpan(), output);
    }

    private static void FormatNull(TextWriter output)
    {
        output.Write("null");
    }

    /// <summary>
    /// 对字符串进行转义
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="output"></param>
    private static void Escape(ReadOnlySpan<char> buffer, TextWriter output)
    {
        const char quote = '"',
            space = ' ',
            backSlash = '\\',
            lineFeed = '\n',
            carriageReturn = '\r',
            formFeed = '\f',
            tab = '\t';

        output.Write(quote);

        var cleanSegmentStart = 0;
        var anyEscaped = false;
        for(var i = 0; i < buffer.Length; i++)
        {
            var c = buffer[i];

            if(c is < space or backSlash or quote)
            {
                anyEscaped = true;

                output.Write(buffer[cleanSegmentStart..i]);

                cleanSegmentStart = i + 1;

                switch(c)
                {
                    case quote:
                        output.Write("\\\"");
                        break;
                    case backSlash:
                        output.Write("\\\\");
                        break;
                    case lineFeed:
                        output.Write("\\n");
                        break;
                    case carriageReturn:
                        output.Write("\\r");
                        break;
                    case formFeed:
                        output.Write("\\f");
                        break;
                    case tab:
                        output.Write("\\t");
                        break;
                    default:
                        output.Write("\\u");
                        output.Write(((int)c).ToString("X4"));
                        break;
                }
            }
        }

        if(anyEscaped)
        {
            if(cleanSegmentStart != buffer.Length)
            {
                output.Write(buffer[cleanSegmentStart..]);
            }
        }
        else
        {
            output.Write(buffer);
        }

        output.Write(quote);
    }
}
