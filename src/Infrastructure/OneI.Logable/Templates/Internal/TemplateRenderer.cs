namespace OneI.Logable;

using OneI.Logable.Rendering;
using OneI.Logable.Templating;
using OneI.Logable.Templating.Properties;

internal static class TemplateRenderer
{
    public static void Render(in TextTemplate context, in TextWriter output, in IReadOnlyList<Token> tokens, in IFormatProvider? formatProvider = null)
    {
        foreach(var token in tokens)
        {
            if(token is PropertyToken propertyToken)
            {
                if(TryRenderProperty(output, propertyToken, context.Properties, formatProvider))
                {
                    continue;
                }
            }

            RenderText(output, token);
        }
    }

    private static void RenderText(TextWriter output, Token token)
    {
        output.Write(token.ToString());
    }

    private static bool TryRenderProperty(
        TextWriter output,
        PropertyToken propertyToken,
        IReadOnlyDictionary<string, PropertyValue> properties,
        IFormatProvider? formatProvider = null)
    {
        if(properties.TryGetValue(propertyToken.Name, out var property) == false)
        {
            if(propertyToken.ParameterIndex.HasValue == false
                || propertyToken.ParameterIndex.Value >= properties.Count)
            {
                return false;
            }

            property = properties.Values.ElementAt(propertyToken.ParameterIndex.Value);
        }

        if(propertyToken.Indent.HasValue)
        {
            output.Write(new string(' ', propertyToken.Indent.Value));
        }

        var writer = propertyToken.Alignment.HasValue ? new StringWriter() : output;

        property.Render(writer, propertyToken.Format, formatProvider);

        if(propertyToken.Alignment.HasValue)
        {
            RenderHelper.Padding(output, ((StringWriter)writer).ToString(), propertyToken.Alignment);
        }

        return true;
    }
}
