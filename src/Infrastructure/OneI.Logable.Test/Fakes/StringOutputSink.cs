namespace OneI.Logable.Fakes;

using OneI.Logable.Templates;

public sealed class StringOutputSink : ILoggerSink
{
    private readonly Action<string> _receiver;

    public StringOutputSink(Action<string> receiver) => _receiver = receiver;

    public void Invoke(in LoggerContext context)
    {
        var writer = new StringWriter();

        TemplateRenderHelper.Render(writer, context, null);

        _receiver.Invoke(writer.ToString());
    }
}
