namespace OneI.Logable;

using OneI.Logable.Templates;

public readonly struct LoggerContext
{
    public LoggerContext(
        TemplateEnumerator template,
        PropertyDictionary properties,
        LoggerMessageContext message)
    {
        Template = template;
        Properties = properties;
        Message = message;
    }

    public readonly TemplateEnumerator Template;

    public readonly PropertyDictionary Properties;

    public readonly LoggerMessageContext Message;
}
