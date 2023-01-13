namespace OneI.Logable.Templatizations;

using System.Collections.Generic;
using OneI.Logable.Templatizations.Tokenizations;

public class TemplateSelector : ITemplateSelector
{
    private readonly string _default;
    private readonly List<Func<LoggerMessageContext, string?>>? _providers;

    public TemplateSelector(string @default, List<Func<LoggerMessageContext, string?>>? providers)
    {
        _providers = providers;
        _default = @default;
    }

    public List<ITemplateToken> Select(in LoggerMessageContext context)
    {
        if(_providers.NotNullOrEmpty())
        {
            foreach(var item in _providers)
            {
                var template = item(context);
                if(template != null)
                {
                    return TemplateParser.Parse(template);
                }
            }
        }

        return TemplateParser.Parse(_default);
    }
}
