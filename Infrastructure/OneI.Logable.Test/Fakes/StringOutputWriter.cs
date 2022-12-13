namespace OneI.Logable.Fakes;

using Templating.Rendering;

public class StringOutputWriter : ILoggerWriter
{
    private readonly StringWriter _writer;
    private readonly ITextRenderer _renderer;

    public StringOutputWriter(string outputTempalte)
    {
        _writer = new();
        _renderer = new TextTemplateRenderer(outputTempalte);
    }

    public void Write(in LoggerContext context)
    {
        _renderer.Render(context, _writer);

        var content = ToString();

        Debug.WriteLine(content, nameof(StringOutputWriter));

        _writer.Flush();
    }

    public override string ToString() => _writer.ToString();
}
