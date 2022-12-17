namespace OneI.Logable.Rendering;

using OneI.Textable;

public class TextRendererProvider : ITextRendererProvider
{
    private readonly TemplateContext _template;
    private readonly IFormatProvider? _formatProvider;

    public TextRendererProvider(string template, IFormatProvider? formatProvider = null)
    {
        _template = TemplateParser.Parse(template);
        _formatProvider = formatProvider;
    }

    public virtual ILoggerRenderer? GetTextRenderer(in LoggerContext context)
    {
        return new LoggerRenderer(_template, _formatProvider);
    }
}

public class ConditionalRendererProvider : ITextRendererProvider
{
    private readonly Func<LoggerContext, bool> _condition;
    private readonly ILoggerRenderer _textRenderer;

    public ConditionalRendererProvider(Func<LoggerContext, bool> condition, ILoggerRenderer textRenderer)
    {
        _condition = condition;
        _textRenderer = textRenderer;
    }

    public virtual ILoggerRenderer? GetTextRenderer(in LoggerContext context)
    {
        if(_condition(context))
        {
            return _textRenderer;
        }

        return null;
    }
}
