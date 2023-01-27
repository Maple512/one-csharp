namespace OneI.Logable.Templatizations;

using System.Collections.Generic;
using OneI.Logable.Templatizations.Tokenizations;

public class TemplateSelector : ITemplateSelector
{
    private readonly TemplateProvider _default;
    private readonly TemplateProvider[] _providers;

    public TemplateSelector(ReadOnlyMemory<char> template, TemplateProvider[] providers)
    {
        var tokens = TemplateParser.Parse(template.Span).ToArray();

        var messageIndex = -1;
        for(var i = 0; i < tokens.Length; i++)
        {
            if(tokens[i] is NamedPropertyToken named and { Name: LoggerConstants.PropertyNames.Message })
            {
                messageIndex = i;
                break;
            }
        }

        _default = new TemplateProvider(DefaultProvider, tokens, messageIndex);

        _providers = providers;
    }

    private static bool DefaultProvider(LoggerMessageContext _) => true;

    public IReadOnlyList<ITemplateToken> Select(LoggerMessageContext context)
    {
        var provider = _default;
        foreach(var item in _providers)
        {
            if(item.IsSupported(context))
            {
                provider = item;
                break;
            }
        }

        if(!provider.HasMessage)
        {
            return provider.Template;
        }

        return TemplateTokenCache.GetOrAdd(context.Message, provider.Template, provider.MessageIndex);
    }
}

public class TemplateProvider
{
    private readonly Func<LoggerMessageContext, bool> _condition;

    public TemplateProvider(Func<LoggerMessageContext, bool> condition, ReadOnlySpan<char> template)
    {
        _condition = condition;

        Template = TemplateParser.Parse(template).ToArray();

        MessageIndex = template.IndexOf(LoggerConstants.MessagePlaceHolder);
    }

    public TemplateProvider(Func<LoggerMessageContext, bool> condition, ITemplateToken[] template, int messageIndex)
    {
        Template = template;
        _condition = condition;
        MessageIndex = messageIndex;
    }

    internal ITemplateToken[] Template { get; }

    internal int MessageIndex { get; }

    internal bool HasMessage
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => MessageIndex != -1;
    }

    public bool IsSupported(LoggerMessageContext context) => _condition(context);
}

internal static class TemplateTokenCache
{
    private static readonly Dictionary<int, ITemplateToken[]> _cache = new(EqualityComparer<int>.Default);

    public static ITemplateToken[] GetOrAdd(ReadOnlyMemory<char> message, ITemplateToken[] template, int messageIndex)
    {
        var key = HashCode.Combine(message.GetHashCode(), template.GetHashCode());

        if(_cache.TryGetValue(key, out var result))
        {
            return result;
        }

        var messageTokens = TemplateParser.Parse(message.Span).ToArray();

        var templateTokens = template.AsSpan();
        var tokens = new ITemplateToken[template.Length + messageTokens.Length - 1];

        scoped var span = tokens.AsSpan();

        templateTokens[..messageIndex].CopyTo(span);
        messageTokens.CopyTo(span[messageIndex..]);
        templateTokens[(messageIndex + 1)..].CopyTo(span[(messageIndex + messageTokens.Length)..]);

        _cache.Add(key, tokens);

        return tokens;
    }
}
