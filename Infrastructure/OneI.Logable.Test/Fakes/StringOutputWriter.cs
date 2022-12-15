namespace OneI.Logable.Fakes;

using Templating.Rendering;

public class StringOutputWriter : ILoggerSink
{
    private readonly StringWriter _writer;
    private readonly ITextRenderer _renderer;

    public StringOutputWriter(string outputTempalte)
    {
        _writer = new();
        _renderer = new TextTemplateRenderer(outputTempalte);
    }

    public void Invoke(in LoggerContext context)
    {
        _renderer.Render(context, _writer);

        _writer.Flush();

        var content = ToString();

        Debug.WriteLine(content, nameof(StringOutputWriter));
    }

    public override string ToString()
    {
        return _writer.ToString();
    }
}
