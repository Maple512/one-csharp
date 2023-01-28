namespace OneI.Logable.Templatizations;

using OneI.Logable.Templatizations.Tokenizations;

internal class TemplateProvider
{
    private readonly TemplateItem _default;
    private readonly TemplateItem[] _providers;

    public TemplateProvider(ReadOnlyMemory<char> template, TemplateItem[] providers)
    {
        var tokens = TemplateParser.Parse(template);

        _default = new TemplateItem(null, tokens);

        _providers = providers;
    }

    public IEnumerable<ITemplateToken> GetTemplate(LoggerMessageContext context)
    {
        TemplateItem template = _default;
        if(_providers is { Length: > 0 })
        {
            foreach(var item in _providers)
            {
                if(item.IsSupported(context))
                {
                    template = item;
                    break;
                }
            }
        }

        return TemplateTokenCache.GetOrAdd(context.Message, template.Template);
    }
}

internal struct TemplateItem
{
    private readonly Func<LoggerMessageContext, bool>? _condition;

    public TemplateItem(Func<LoggerMessageContext, bool> condition, ReadOnlyMemory<char> template)
    {
        _condition = condition;
        Template = TemplateParser.Parse(template);
    }

    internal TemplateItem(Func<LoggerMessageContext, bool>? condition, IEnumerable<ITemplateToken> template)
    {
        Template = template;
        _condition = condition;
    }

    internal IEnumerable<ITemplateToken> Template
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    public bool IsSupported(LoggerMessageContext context) => _condition is null || _condition(context);
}

internal static class TemplateTokenCache
{
    private static readonly ValueDictionary<int, IEnumerable<ITemplateToken>> _cache = new(4);

    public static IEnumerable<ITemplateToken> GetOrAdd(ReadOnlyMemory<char> message, IEnumerable<ITemplateToken> template)
    {
        var key = HashCode.Combine(message.GetHashCode(), template.GetHashCode());

        if(_cache.TryGetValue(key, out var result))
        {
            return result;
        }

        var tokens = AppendMessage(message, template);

        _cache.Add(key, tokens);

        return tokens;
    }

    private static IEnumerable<ITemplateToken> AppendMessage(ReadOnlyMemory<char> message, IEnumerable<ITemplateToken> template)
    {
        var messageTokens = TemplateParser.Parse(message);

        foreach(var token in template)
        {
            if(token is NamedPropertyToken named and { Name: LoggerConstants.PropertyNames.Message })
            {
                foreach(var messageItemToken in messageTokens)
                {
                    yield return messageItemToken;
                }
            }
            else
            {
                yield return token;
            }
        }

        yield break;
    }
}
