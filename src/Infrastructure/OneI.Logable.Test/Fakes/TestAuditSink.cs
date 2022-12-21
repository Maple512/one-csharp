namespace OneI.Logable.Fakes;

using OneI.Textable.Templating.Properties;

public class TestAuditSink : ILoggerSink
{
    private static IReadOnlyDictionary<string, PropertyValue>? _properties;

    public void Invoke(in LoggerContext context)
    {
        _properties = context.Properties;
    }

    public static IReadOnlyDictionary<string, PropertyValue> Properties
        => _properties ?? new Dictionary<string, PropertyValue>();
}
