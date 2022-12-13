namespace OneI.Logable.Templating.Rendering;

using Properties;

public static class MessageTemplateRender
{
    public static void Render(
        TextTemplate template,
        IReadOnlyDictionary<string, PropertyValue> properties,
        TextWriter writer,
        string? format = null,
        IFormatProvider? formatProvider = null)
    {
        foreach(var item in template.Tokens)
        {
            if(item is TextToken tt)
            {
                TextTemplateRenderer.RenderText(writer, tt);
            }
            else
            {
                RenderPropertyToken((PropertyToken)item,
                    properties,
                    writer,
                    formatProvider);
            }
        }
    }

    public static void RenderPropertyToken(
        PropertyToken pt,
        IReadOnlyDictionary<string, PropertyValue> properties,
        TextWriter output,
        IFormatProvider? formatProvider)
    {
        if(!properties.TryGetValue(pt.Name, out var propertyValue))
        {
            output.Write(pt.Text);
            return;
        }

        if(!pt.Alignment.HasValue)
        {
            RenderValue(propertyValue, output, pt.Format, formatProvider);
            return;
        }

        var valueOutput = new StringWriter();

        RenderValue(propertyValue, valueOutput, pt.Format, formatProvider);

        var value = valueOutput.ToString();

        if(value.Length >= pt.Alignment.Value.Width)
        {
            output.Write(value);
            return;
        }

        RenderHelper.Padding(output, value, pt.Alignment.Value);
    }

    private static void RenderValue(
        PropertyValue propertyValue,
        TextWriter output,
        string? format,
        IFormatProvider? formatProvider) => propertyValue.Render(output, format, formatProvider);
}
