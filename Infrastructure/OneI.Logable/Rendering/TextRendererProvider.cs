namespace OneI.Logable.Rendering;

public class TextRendererProvider : ITextRendererProvider
{
    private readonly string _template;
    private readonly IFormatProvider? _formatProvider;

    public TextRendererProvider(string template, IFormatProvider? formatProvider = null)
    {
        _template = template;
        _formatProvider = formatProvider;
    }

    public virtual ITextRenderer? GetTextRenderer(in LoggerContext context)
    {
        return new TextTemplateRenderer(_template, _formatProvider);
    }
}

public class ConditionalRendererProvider : ITextRendererProvider
{
    private readonly Func<LoggerContext, bool> _condition;
    private readonly ITextRenderer _textRenderer;

    public ConditionalRendererProvider(Func<LoggerContext, bool> condition, ITextRenderer textRenderer)
    {
        _condition = condition;
        _textRenderer = textRenderer;
    }

    public virtual ITextRenderer? GetTextRenderer(in LoggerContext context)
    {
        if(_condition(context))
        {
            return _textRenderer;
        }

        return null;
    }
}
