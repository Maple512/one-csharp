namespace OneI.Logable.Fakes;

using Templating.Rendering;

public class StringOutputWriter : ILoggerEndpoint
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

        var content = ToString();

        Debug.WriteLine(content, nameof(StringOutputWriter));

        _writer.Flush();
    }

    public override string ToString()
    {
        return _writer.ToString();
    }
}
