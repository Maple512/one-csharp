namespace OneI.Logable.Templates;

internal class TemplateProvider
{
    private readonly TemplateItem _default;
    private readonly TemplateItem[] _providers;

    public TemplateProvider(TemplateItem template, TemplateItem[] providers)
    {
        _default = template;
        _providers = providers;
    }

    public LoggerTemplateEnumerator GetTemplate(ref LoggerMessageContext context)
    {
        var template = _default;
        if(_providers is { Length: > 0, })
        {
            foreach(var item in _providers)
            {
                if(item.Condition?.Invoke(context) == true)
                {
                    template = item;
                    break;
                }
            }
        }

        return TemplateTokenCache.GetOrAdd(ref template, ref context);
    }
}

internal readonly struct TemplateItem
{
    public readonly TemplateHolder[] Holders;

    public readonly Func<LoggerMessageContext, bool>? Condition;
    public readonly int MessageIndex;

    public TemplateItem(Func<LoggerMessageContext, bool>? condition, ReadOnlyMemory<char> template)
    {
        Holders = new TemplateQueue(template).ToArray();
        for(var i = 0;i < Holders.Length;i++)
        {
            if(Holders[i].Name?.Equals(LoggerConstants.Propertys.Message, StringComparison.InvariantCulture) == true)
            {
                MessageIndex = i;
                break;
            }
        }

        Condition = condition;
    }

    public override int GetHashCode() => HashCode.Combine(Holders, Condition, MessageIndex);
}

internal static class TemplateTokenCache
{
    private static readonly Dictionary<int, LoggerTemplateEnumerator> _cache = new(20);

    public static LoggerTemplateEnumerator GetOrAdd(ref TemplateItem template, ref LoggerMessageContext context)
    {
        var key = HashCode.Combine(template.GetHashCode(), context.Message.GetHashCode());

        if(_cache.TryGetValue(key, out var value))
        {
            return value;
        }

        value = new LoggerTemplateEnumerator(template.Holders, template.MessageIndex, new TemplateEnumerator(context.Message.AsMemory()));

        _cache.Add(key, value);

        return value;
    }
}
