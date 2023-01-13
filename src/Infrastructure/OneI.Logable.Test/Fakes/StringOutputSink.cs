namespace OneI.Logable.Fakes;

using OneI.Logable.Rendering;
using OneI.Logable;

public sealed class StringOutputSink : ILoggerSink
{
    private readonly ILoggerRenderer _renderer;

    public StringOutputSink(string tempalte)
    {
        _renderer = new LoggerRenderer(null);
    }

    public void Invoke(in LoggerContext context)
    {
        var _writer = new StringWriter();

        _renderer.Render(context, _writer);

        Debug.Write(_writer.ToString(), nameof(StringOutputSink));
    }
}
