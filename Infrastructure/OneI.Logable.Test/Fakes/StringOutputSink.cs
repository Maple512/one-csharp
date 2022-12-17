namespace OneI.Logable.Fakes;

using OneI.Logable.Rendering;
using OneI.Textable;

public sealed class StringOutputSink : ILoggerSink
{
    private readonly StringWriter _writer;
    private readonly ILoggerRenderer _renderer;

    public StringOutputSink(string tempalte)
    {
        _writer = new();
        _renderer = new LoggerRenderer(TemplateParser.Parse(tempalte));
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
