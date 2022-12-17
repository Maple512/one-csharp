namespace OneI.Logable.Fakes;

using OneI.Textable.Templating.Properties;

public class TestAuditSink : ILoggerSink
{
    public static IReadOnlyDictionary<string, PropertyValue> Properties;

    public void Invoke(in LoggerContext context)
    {
        Properties = context.Properties;
    }
}
