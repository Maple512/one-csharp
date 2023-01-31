namespace OneI.Logable.Templates;

using Cysharp.Text;

internal class TemplateProvider
{
    private readonly ReadOnlyMemory<char> _default;
    private readonly TemplateItem[] _providers;

    public TemplateProvider(ReadOnlyMemory<char> template, TemplateItem[] providers)
    {
        _default = template;

        _providers = providers;
    }

    public TemplateEnumerator GetTemplate(ref LoggerMessageContext context)
    {
        var template = _default;
        if(_providers is { Length: > 0 })
        {
            foreach(var item in _providers)
            {
                if(item.IsSupported(ref context))
                {
                    template = item.Template;
                    break;
                }
            }
        }

        var builder = ZString.CreateStringBuilder(true);

        try
        {
            builder.Append(template);
            builder.Replace(LoggerConstants.Propertys.Message, template.Span);

            return TemplateTokenCache.GetOrAdd(builder);
        }
        finally
        {
            builder.Dispose();
        }
    }
}

internal readonly struct TemplateItem
{
    private readonly Func<LoggerMessageContext, bool>? _condition;

    public TemplateItem(Func<LoggerMessageContext, bool> condition, ReadOnlyMemory<char> template)
    {
        _condition = condition;
        Template = template;
    }

    internal ReadOnlyMemory<char> Template
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSupported(ref LoggerMessageContext context) => _condition is null || _condition(context);
}

internal static class TemplateTokenCache
{
    private static readonly Dictionary<int, TemplateEnumerator> _cache = new(4);

    public static TemplateEnumerator GetOrAdd(in Utf16ValueStringBuilder builder)
    {
        var key = builder.GetHashCode();
        if(_cache.TryGetValue(key, out var value))
        {
            return value;
        }

        value = new TemplateEnumerator(builder.AsMemory());

        _cache.Add(key, value);

        return value;
    }

    //public static LoggerTemplateEnumerator GetOrAdd(string message, ReadOnlyMemory<char> template)
    //{
    //    var chche = _cache;
    //    var key = HashCode.Combine(message.GetHashCode(), template.GetHashCode());

    //    if(chche.TryGetValue(key, out var result))
    //    {
    //        return result;
    //    }

    //    var value = new LoggerTemplateEnumerator(template, new TemplateEnumerator(message.AsMemory()));

    //    chche.Add(key, value);

    //    return value;
    //}
}
