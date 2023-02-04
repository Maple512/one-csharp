namespace OneI.Logable.Templates;

using System.Globalization;
using Cysharp.Text;
using OneI.Logable.Formatters;
using static LoggerConstants.Propertys;

public static class TemplateRenderHelper
{
    public static void Render(TextWriter writer, in LoggerContext context, IFormatProvider? formatProvider)
    {
        var template = context.Template;
        var message = context.Message;
        var properties = context.Properties;

        PropertyValue value = default;
        var container = ZString.CreateStringBuilder(true);

        formatProvider ??= CultureInfo.InvariantCulture;

        var argumentIndex = 0;
        // 350us
        while(template.MoveNext())
        {
            // 800us
            var holder = template.Current;

            if(holder.Indent > 0)
            {
                container.Append(' ', holder.Indent);
            }

            if(holder is { Length: > 0 })
            {
                container.Append(holder.Text.Span);
            }
            else if(holder.IsIndexer())
            {
                if(properties.Length > holder.ParameterIndex)
                {
                    value = properties[holder.ParameterIndex].Value;
                    argumentIndex++;
                }

                LiteralRender(ref container, value.Value, ref holder, formatProvider);
            }
            else if(holder.IsNamed())
            {
                PropertyRender(ref container, ref holder, ref message, ref properties, ref argumentIndex, formatProvider);
            }

            var written = container.Length;
            if(written > 0)
            {
                if(written >= holder.Align)
                {
                    writer.Write(container.AsSpan());
                }
                else
                {
                    if(holder.Align < 0)
                    {
                        writer.Write(container.AsSpan());
                    }

                    var pad = Math.Abs(holder.Align) - written;
                    WriteSpance(writer, pad);

                    if(holder.Align > 0)
                    {
                        writer.Write(container.AsSpan());
                    }
                }

                container.Clear();
            }
        }

        container.Dispose();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void WriteSpance(TextWriter writer, int length)
    {
        scoped Span<char> spaces = stackalloc char[length];
        spaces.Fill(' ');
        writer.Write(spaces);
    }

    private static void PropertyRender(
        ref Utf16ValueStringBuilder writer,
        ref TemplateHolder holder,
        ref LoggerMessageContext message,
        ref PropertyDictionary properties,
        ref int argumentIndex,
        IFormatProvider formatProvider)
    {
        if(holder.Name == Level)
        {
            var level = LevelFormatHelper.Format(message.Level, holder.Format);

            writer.Append(level);
            return;
        }

        if(holder.Name == NewLine)
        {
            writer.AppendLine();
            return;
        }

        if(holder.Name == Exception)
        {
            if(message.Exception is not null)
            {
                writer.Append(message.Exception.ToString());
            }

            return;
        }

        if(holder.Name == Timestamp)
        {
            WriteSpanFormattable(ref writer, message.Timestamp, holder.Format, formatProvider);
            return;
        }

        if(holder.Name == File)
        {
            writer.Append(message.File);
            return;
        }

        if(holder.Name == Member)
        {
            writer.Append(message.Member);
            return;
        }

        if(holder.Name == Line)
        {
            WriteSpanFormattable(ref writer, message.Line, holder.Format, formatProvider);

            return;
        }

        if(!properties.TryGetValue(holder.Name!, out var propertyValue))
        {
            if(properties.Length > argumentIndex)
            {
                propertyValue = properties[argumentIndex++].Value;
            }
            else
            {
                writer.Append(TemplateConstants.Property.Null);
                return;
            }
        }

        LiteralRender(ref writer, propertyValue.Value.Value, ref holder, formatProvider);
    }

    internal static void LiteralRender<TValue>(
        ref Utf16ValueStringBuilder writer,
        TValue? value,
        ref TemplateHolder holder,
        IFormatProvider formatProvider)
    {
        if(value == null)
        {
            writer.Append(TemplateConstants.Property.Null);
            return;
        }

        if(value is IPropertyValueFormattable pf)
        {
            scoped var destination = writer.GetSpan(0);
            if(pf.TryFormat(destination, holder.Type, out var written))
            {
                writer.Advance(written);
                return;
            }
        }

        if(value is string str)
        {
            if(holder.Type == PropertyType.Stringify)
            {
                writer.Append('"');
                writer.Append(str.Replace("\"", "\\\""));
                writer.Append('"');
            }
            else
            {
                writer.Append(str);
            }

            return;
        }

        if(value is ISpanFormattable sf)
        {
            WriteSpanFormattable(ref writer, sf, holder.Format, formatProvider);

            return;
        }

        if(value is IFormattable f)
        {
            writer.Append(f.ToString(holder.Format, formatProvider));
            return;
        }

        var custom = (ICustomFormatter?)formatProvider.GetFormat(typeof(ICustomFormatter));
        if(custom != null)
        {
            writer.Append(custom.Format(holder.Format, value, formatProvider));
            return;
        }

        writer.Append(value.ToString()!);
    }

    private const int maxBufferSize = int.MaxValue / 2;

    private static void WriteSpanFormattable(
        ref Utf16ValueStringBuilder writer,
        ISpanFormattable spanFormattable,
        string format,
        IFormatProvider formatProvider)
    {
        scoped var buffer = writer.GetSpan(0);

        if(spanFormattable.TryFormat(buffer, out var charsWritten, format, formatProvider))
        {
            writer.Advance(charsWritten);
        }
    }
}
