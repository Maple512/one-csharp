namespace OneI.Logable.Templatizations;

using OneI.Logable.Formatters;
using Tokenizations;
using static LoggerConstants.PropertyNames;

public static class TemplateRenderer
{
    public static void Render(
        TextWriter writer,
        IEnumerable<ITemplateToken> tokens,
        LoggerMessageContext messageContext,
        IFormatProvider? formatProvider = null)
    {
        foreach(var token in tokens)
        {
            if(token is TextToken tt)
            {
                TextRender(writer, tt);
                continue;
            }

            ITemplatePropertyValue? value = null;
            if(token is IndexerPropertyToken indexer)
            {
                if(messageContext.Properties.Length > indexer.ParameterIndex)
                {
                    value = messageContext.Properties[indexer.ParameterIndex].Value;
                }

                PropertyRender(writer, (ITemplatePropertyToken)token, value, formatProvider);

                continue;
            }

            if(token is NamedPropertyToken named)
            {
                RenderNamedProperty(writer, named, messageContext, formatProvider);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void TextRender(TextWriter writer, TextToken token) => writer.Write(token.Text);

    private static void RenderNamedProperty(
        TextWriter writer,
        NamedPropertyToken token,
        LoggerMessageContext context,
        IFormatProvider? formatProvider)
    {
        if(token.Indent is not null)
        {
            WriteWhiteSpace(writer, token.Indent.Value);
        }

        if(token.Name == Level)
        {
            var level = LevelFormatHelper.Format(context.Level, token.Format);

            Padding(writer, level, token.Alignment);
        }
        else if(token.Name == NewLine)
        {
            Padding(writer, Environment.NewLine, token.Alignment);
        }
        else if(token.Name == Exception)
        {
            Padding(writer, context.Exception?.ToString() ?? string.Empty, token.Alignment);
        }
        else
        {
            var w = token.Alignment.HasValue ? new StringWriter() : writer;

            if(token.Name == Timestamp)
            {
                TemplateRenderHelper.Render(context.Timestamp, w, token.Type, token.Format, null, formatProvider);
            }
            else
            {
                if(!context.Properties.TryGetValue(token.Name, out var templatePropertyValue))
                {
                    writer.Write("[Null]");
                    return;
                }

                templatePropertyValue.Render(w, token.Type, token.Format, formatProvider);
            }

            if(token.Alignment.HasValue)
            {
                Padding(writer, ((StringWriter)w).ToString(), token.Alignment);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PropertyRender(
        TextWriter writer,
        ITemplatePropertyToken token,
        ITemplatePropertyValue? value,
        IFormatProvider? formatProvider)
    {
        if(value == null)
        {
            writer.Write("[Null]");
        }
        else
        {
            if(token.Indent is not null)
            {
                WriteWhiteSpace(writer, token.Indent.Value);
            }

            var w = token.Alignment.HasValue ? new StringWriter() : writer;

            value.Render(writer, token.Type, token.Format, formatProvider);

            if(token.Alignment.HasValue)
            {
                var align = token.Alignment.Value;

                var valueStr = ((StringWriter)w).ToString();

                if(valueStr.Length >= align.Width)
                {
                    writer.Write(valueStr);
                    return;
                }

                var pad = align.Width - valueStr.Length;

                if(align.Direction == TextDirection.Left)
                {
                    writer.Write(valueStr);
                }

                WriteWhiteSpace(writer, pad);

                if(align.Direction == TextDirection.Right)
                {
                    writer.Write(valueStr);
                }
            }
        }
    }

    private static void Padding(TextWriter writer, string value, TextAlignment? align)
    {
        if(align is null || value.Length >= align.Value.Width)
        {
            writer.Write(value);
            return;
        }

        var pad = align.Value.Width - value.Length;

        if(align.Value.Direction == TextDirection.Left)
        {
            writer.Write(value);
        }

        WriteWhiteSpace(writer, pad);

        if(align.Value.Direction == TextDirection.Right)
        {
            writer.Write(value);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void WriteWhiteSpace(TextWriter writer, int length)
    {
        scoped Span<char> span = stackalloc char[length];
        span.Fill(' ');
        writer.Write(span);
    }
}
