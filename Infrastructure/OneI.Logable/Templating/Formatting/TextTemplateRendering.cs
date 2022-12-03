namespace OneI.Logable.Templating.Formatting;

using System;

public class TextTemplateRendering
{
    private readonly TextTemplate _template;
    private readonly IFormatProvider? _formatProvider;

    public TextTemplateRendering(string template, IFormatProvider? formatProvider = null)
    {
        var tokens = TextParser.Parse(template);

        _formatProvider = formatProvider;

        _template = new(template, tokens);
    }

    public void Render(LoggerContext context, TextWriter writer)
    {
        foreach(var token in _template.Tokens)
        {
            switch(token)
            {
                case TextToken textToken:
                    FormatText(textToken, writer);
                    break;
                case PropertyToken propertyToken:
                    FormatProperty(propertyToken, context, writer);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown token type: {token}");
            }
        }
    }

    private static void FormatText(TextToken token, TextWriter writer) => writer.Write(token.ToString());

    private static void FormatProperty(PropertyToken token, LoggerContext context, TextWriter writer)
    {

    }
}
