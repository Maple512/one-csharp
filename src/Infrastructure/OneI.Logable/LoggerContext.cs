namespace OneI.Logable;

using OneI.Logable.Templates;

public readonly struct LoggerContext
{
    public LoggerContext(ref LoggerTemplateEnumerator template
                         , ref PropertyDictionary properties
                         , ref LoggerMessageContext message)
    {
        Template = template;
        Properties = properties;
        Message = message;
    }

    public readonly LoggerTemplateEnumerator Template;

    public readonly PropertyDictionary Properties;

    public readonly LoggerMessageContext Message;
}
