namespace OneI.Logable.Templatizations;

using System.Collections.Generic;
using OneI.Logable.Templatizations.Tokenizations;

public class TemplateSelector : ITemplateSelector
{
    private readonly Dictionary<string, List<ITemplateToken>> _cache = new(5, EqualityComparer<string>.Default);

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
                var template = item.Invoke(context);
                if(template != null)
                {
                    return _cache.GetOrAdd(template, TemplateParser.Parse(template));
                }
            }
        }

        return _cache.GetOrAdd(_default, TemplateParser.Parse(_default));
    }
}
