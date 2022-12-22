namespace OneI.Textable;

using OneI.Textable.Rendering;
using OneI.Textable.Templating;
/// <summary>
/// The template renderer.
/// </summary>

public static class TemplateRenderer
{
    /// <summary>
    /// Renders the.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="output">The output.</param>
    /// <param name="options">The options.</param>
    public static void Render(in TemplateContext context, in TextWriter output, in TemplateOptions options)
    {
        foreach(var token in context.Tokens)
        {
            if(token is PropertyToken propertyToken)
            {
                if(TryRenderProperty(output, propertyToken, options))
                {
                    continue;
                }
            }

            RenderText(output, token);
        }
    }

    /// <summary>
    /// Renders the text.
    /// </summary>
    /// <param name="output">The output.</param>
    /// <param name="token">The token.</param>
    private static void RenderText(TextWriter output, Token token)
    {
        output.Write(token.ToString());
    }

    /// <summary>
    /// Tries the render property.
    /// </summary>
    /// <param name="output">The output.</param>
    /// <param name="token">The token.</param>
    /// <param name="options">The options.</param>
    /// <returns>A bool.</returns>
    private static bool TryRenderProperty(TextWriter output, PropertyToken token, TemplateOptions options)
    {
        if(options.Properties.TryGetValue(token.Name, out var property) == false)
        {
            if(token.ParameterIndex.HasValue == false
                || token.ParameterIndex.Value >= options.Properties.Count)
            {
                return false;
            }

            property = options.Properties.Values.ElementAt(token.ParameterIndex.Value);
        }

        if(token.Indent != null)
        {
            output.Write(new string(' ', token.Indent.Value));
        }

        var writer = token.Alignment.HasValue ? new StringWriter() : output;

        property.Render(writer, token.Format, options.FormatProvider);

        if(token.Alignment.HasValue)
        {
            RenderHelper.Padding(output, ((StringWriter)writer).ToString(), token.Alignment);
        }

        return true;
    }
}
