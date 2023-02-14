namespace OneI.Logable;
using OneI.Logable.Templates;

public struct LoggerContext
{
    public LoggerContext(ref LoggerTemplateEnumerator template, ref PropertyDictionary properties, ref LoggerMessageContext message)
    {
        Template = template;
        Properties = properties;
        Message = message;
    }

    public readonly LoggerTemplateEnumerator Template;

    public readonly PropertyDictionary Properties;

    public readonly LoggerMessageContext Message;

    private string? _sourceContext;

    public string SourceContext
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            _sourceContext ??= Properties.GetValue<string>(LoggerConstants.Propertys.SourceContext)
                    ?? LoggerConstants.Propertys.DefaultSourceContext;

            return _sourceContext;
        }
    }

    private int? eventId;

    public int EventId
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            eventId ??= Properties.GetValue<int?>(LoggerConstants.Propertys.EventId)
                ?? LoggerConstants.Propertys.DefaultEventId;

            return eventId.Value;
        }
    }
}
