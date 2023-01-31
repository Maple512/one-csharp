namespace OneI.Logable.Templates;

using System.Globalization;
using OneI.Logable.Formatters;
using OneI.Text;
using static LoggerConstants.Propertys;

public static class TemplateRenderHelper
{
    internal static void Render(
        TextWriter writer,
        TemplateEnumerator template,
        in LoggerMessageContext message,
        in PropertyDictionary properties,
        IFormatProvider? formatProvider)
    {
        PropertyValue value = default;
        var container = new ValueStringWriter();

        while(template.MoveNext())
        {
            var length = container.Length;

            var holder = template.Current;

            if(holder.Indent > 0)
            {
                writer.Write(new string(' ', holder.Indent));
            }

            if(holder.IsText())
            {
                container.Write(template.GetCurrentText().Span);
            }
            else if(holder.IsIndexer())
            {
                if(properties.Length > holder.ParameterIndex)
                {
                    value = properties[holder.ParameterIndex].Value;
                }

                LiteralRender(container, value.Value, holder.Type, holder.Format, formatProvider);
            }
            else if(holder.IsNamed())
            {
                PropertyRender(container, holder, message, properties, formatProvider);
            }

            var written = container.Length - length;

            if(written >= holder.Align)
            {
                writer.Write(container.Span);
            }
            else
            {
                if(holder.Align < 0)
                {
                    writer.Write(container.Span);
                }

                var pad = Math.Abs(holder.Align) - written;

                writer.Write(new string(' ', pad));

                if(holder.Align > 0)
                {
                    writer.Write(container.Span);
                }
            }

            container.Clear();
        }

        container.Dispose();
    }

    private static void PropertyRender(
        TextWriter writer,
        in TemplateHolder token,
        in LoggerMessageContext message,
        in PropertyDictionary properties,
        IFormatProvider? formatProvider)
    {
        if(token.Name == Level)
        {
            var level = LevelFormatHelper.Format(message.Level, token.Format);

            writer.Write(level);
        }
        else if(token.Name == NewLine)
        {
            writer.WriteLine();
        }
        else if(token.Name == Exception)
        {
            writer.Write(message.Exception?.ToString() ?? string.Empty);
        }
        else if(token.Name == Timestamp)
        {
            LiteralRender(writer, message.Timestamp, token.Type, token.Format, formatProvider);
        }
        else if(token.Name == File)
        {
            writer.Write(message.File);
        }
        else if(token.Name == Member)
        {
            writer.Write(message.Member);
        }
        else if(token.Name == Line)
        {
            LiteralRender(writer, message.Line, token.Type, token.Format, formatProvider);
        }
        else
        {
            if(!properties.TryGetValue(token.Name!, out var propertyValue))
            {
                writer.Write(TemplateConstants.Property.Null);
                return;
            }

            LiteralRender(writer, propertyValue, token.Type, token.Format, formatProvider);
        }
    }

    internal static void LiteralRender<TValue>(
       TextWriter writer,
       TValue? value,
       PropertyType type,
       string? format,
       IFormatProvider? formatProvider)
    {
        if(value == null)
        {
            writer.Write(TemplateConstants.Property.Null);
            return;
        }

        if(value is string str)
        {
            if(type == PropertyType.Stringify)
            {
                writer.Write("\"");
                writer.Write(str.Replace("\"", "\\\""));
                writer.Write("\"");
            }
            else
            {
                writer.Write(str);
            }

            return;
        }

        if(value is IFormattable f)
        {
            writer.Write(f.ToString(format, formatProvider ?? CultureInfo.InvariantCulture));
            return;
        }

        var custom = (ICustomFormatter?)formatProvider?.GetFormat(typeof(ICustomFormatter));
        if(custom != null)
        {
            writer.Write(custom.Format(format, value, formatProvider));
            return;
        }

        writer.Write(value.ToString());
    }
}
