namespace OneI.Logable.Templates;

using OneI.Logable;

internal class TemplateProvider
{
    internal readonly TemplateItem _default;
    internal readonly TemplateItem[] _providers;

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

        var messageTemplate = new TemplateEnumerator(context.Message.AsMemory());

        return new LoggerTemplateEnumerator(template.Holders, template.MessageIndex, messageTemplate);
    }
}

public readonly struct TemplateItem
{
    public readonly TemplateHolder[] Holders;

    public readonly Func<LoggerMessageContext, bool>? Condition;
    public readonly int MessageIndex;

    public TemplateItem(Func<LoggerMessageContext, bool>? condition, ReadOnlyMemory<char> template)
    {
        Holders = new TemplateQueue(template).ToArray();
        for(var i = 0; i < Holders.Length; i++)
        {
            if(Holders[i].Name?.Equals(LoggerConstants.Propertys.Message, StringComparison.InvariantCulture) == true)
            {
                MessageIndex = i;
                break;
            }
        }

        Condition = condition;
    }
}
