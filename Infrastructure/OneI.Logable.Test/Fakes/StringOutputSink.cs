namespace OneI.Logable.Fakes;

using OneI.Logable.Rendering;

public sealed class StringOutputSink : ILoggerSink
{
    private readonly StringWriter _writer;
    private readonly ITextRenderer _renderer;

    public StringOutputSink(string outputTempalte)
    {
        _writer = new();
        _renderer = new TextTemplateRenderer(outputTempalte);
    }

    public void Invoke(in LoggerContext context)
    {
        _renderer.Render(context, _writer);

        _writer.Flush();

        Debug.WriteLine(_writer.ToString(), nameof(StringOutputSink));
    }

    public override string ToString()
    {
        return _writer.ToString();
    }
}
