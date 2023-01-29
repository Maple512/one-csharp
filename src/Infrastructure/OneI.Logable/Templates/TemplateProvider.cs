namespace OneI.Logable.Templates;

internal class TemplateProvider
{
    private readonly TemplateItem _default;
    private readonly TemplateItem[] _providers;

    public TemplateProvider(ReadOnlyMemory<char> template, TemplateItem[] providers)
    {
        _default = new TemplateItem(null, new TemplateEnumerator(template.ToString()));

        _providers = providers;
    }

    public MergeTemplateEnumerator GetTemplate(LoggerMessageContext context)
    {
        var template = _default;
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

internal readonly struct TemplateItem
{
    private readonly Func<LoggerMessageContext, bool>? _condition;

    public TemplateItem(Func<LoggerMessageContext, bool> condition, ReadOnlyMemory<char> template)
    {
        _condition = condition;
        Template = new TemplateEnumerator(template.ToString());
    }

    internal TemplateItem(Func<LoggerMessageContext, bool>? condition, TemplateEnumerator template)
    {
        Template = template;
        _condition = condition;
    }

    internal TemplateEnumerator Template
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSupported(LoggerMessageContext context) => _condition is null || _condition(context);
}

internal static class TemplateTokenCache
{
    private static readonly Dictionary<int, MergeTemplateEnumerator> _cache = new(4);

    public static MergeTemplateEnumerator GetOrAdd(string message, TemplateEnumerator template)
    {
        var chche = _cache;
        var key = HashCode.Combine(message.GetHashCode(), template.GetHashCode());

        if(chche.TryGetValue(key, out var result))
        {
            return result;
        }

        var value = new MergeTemplateEnumerator(template, new TemplateEnumerator(message.ToString()));

        chche.Add(key, value);

        return value;
    }
}

internal readonly struct MergeTemplateEnumerator
{
    public readonly TemplateEnumerator Template;
    public readonly TemplateEnumerator Message;

    public MergeTemplateEnumerator(TemplateEnumerator template, TemplateEnumerator message)
    {
        Template = template;
        Message = message;
    }
}
