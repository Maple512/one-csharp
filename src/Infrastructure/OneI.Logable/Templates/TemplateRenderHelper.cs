namespace OneI.Logable.Templates;

using System.Globalization;
using DotNext;
using OneI.Logable.Formatters;
using static LoggerConstants.PropertyNames;

public static class TemplateRenderHelper
{
    internal static void Render(
        TextWriter writer,
        TemplateEnumerator template,
        TemplateEnumerator message,
        LoggerMessageContext context,
        IFormatProvider? formatProvider)
    {
        var properties = context.Properties;

        while(template.MoveNext())
        {
            var holder = template.Current;

            if(holder.IsText())
            {
                scoped var text = template.Template.AsSpan().Slice(holder.Position, holder.Length);

                writer.Write(text);
            }
            else if(holder.IsIndexer())
            {
                ITemplatePropertyValue? value = null;
                if(properties.Length > holder.ParameterIndex)
                {
                    value = properties[holder.ParameterIndex].Value;
                }

                PropertyValueRender(writer, holder, value, formatProvider);
            }
            else if(holder.IsNamed())
            {
                PropertyRender(writer, holder, context, message, formatProvider);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RenderMessage(
        TextWriter writer,
        TemplateEnumerator template,
        LoggerMessageContext context,
        IFormatProvider? formatProvider)
    {
        Render(writer, template, default, context, formatProvider);
    }

    private static void PropertyRender(
        TextWriter writer,
        TemplateHolder token,
        LoggerMessageContext context,
        TemplateEnumerator message,
        IFormatProvider? formatProvider)
    {
        if(token.Indent > 0)
        {
            writer.Write(new string(' ', token.Indent));
        }

        if(token.Name == Level)
        {
            var level = LevelFormatHelper.Format(context.Level, token.Format);

            Padding(writer, level, token.Align);
        }
        else if(token.Name == NewLine)
        {
            Padding(writer, Environment.NewLine, token.Align);
        }
        else if(token.Name == Exception)
        {
            Padding(writer, context.Exception?.ToString() ?? string.Empty, token.Align);
        }
        else
        {
            var w = token.Align != 0 ? new StringWriter() : writer;

            if(token.Name == Timestamp)
            {
                LiteralRender(context.Timestamp, w, token.Type, token.Format, formatProvider);
            }
            else if(token.Name == Message && message.Equals(default) == false)
            {
                RenderMessage(w, message, context, formatProvider);
            }
            else
            {
                if(!context.Properties.TryGetValue(token.Name!, out var templatePropertyValue))
                {
                    writer.Write(TemplateConstants.Property.Null);
                    return;
                }

                templatePropertyValue.Render(w, token.Type, token.Format, formatProvider);
            }

            if(token.Align != 0)
            {
                Padding(writer, ((StringWriter)w).ToString(), token.Align);
            }
        }
    }

    private static void PropertyValueRender(
        TextWriter writer,
        TemplateHolder token,
        ITemplatePropertyValue? value,
        IFormatProvider? formatProvider)
    {
        if(value == null)
        {
            writer.Write(TemplateConstants.Property.Null);
            return;
        }

        if(token.Indent != 0)
        {
            writer.Write(new string(' ', token.Indent));
        }

        var w = token.Align != 0 ? new StringWriter() : writer;

        value.Render(writer, token.Type, token.Format, formatProvider);

        if(token.Align != 0)
        {
            var internalWriter = ((StringWriter)w).GetStringBuilder();

            if(internalWriter.Length >= token.Align)
            {
                writer.Write(internalWriter);
                return;
            }

            var pad = token.Align - internalWriter.Length;

            if(token.Align < 0)
            {
                writer.Write(internalWriter);
            }

            writer.Write(new string(' ', pad));

            if(token.Align > 0)
            {
                writer.Write(internalWriter);
            }
        }
    }

    private static void Padding(TextWriter writer, string value, sbyte align)
    {
        if(align != 0 || value.Length >= align)
        {
            writer.Write(value);
            return;
        }

        var pad = align - value.Length;

        if(align < 0)
        {
            writer.Write(value);
        }

        writer.Write(new string(' ', pad));

        if(align > 0)
        {
            writer.Write(value);
        }
    }

    internal static void LiteralRender<TValue>(
       TValue? value,
       TextWriter writer,
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

        var custom = (ICustomFormatter?)formatProvider?.GetFormat(typeof(ICustomFormatter));
        if(custom != null)
        {
            writer.Write(custom.Format(format, value, formatProvider));
            return;
        }

        if(value is IFormattable f)
        {
            writer.Write(f.ToString(format, formatProvider ?? CultureInfo.InvariantCulture));
            return;
        }

        writer.Write(value.ToString());
    }
}
