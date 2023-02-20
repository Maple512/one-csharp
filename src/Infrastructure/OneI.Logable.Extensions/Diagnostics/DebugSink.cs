namespace OneI.Logable.Diagnostics;

using Cysharp.Text;
using OneI.Logable.Templates;

internal class DebugSink : ILoggerSink
{
    public void Invoke(in LoggerContext context)
    {
        using var writer = new ZStringWriter();

        context.WriteTo(writer);

        Debug.WriteLine(writer.ToString(), context.SourceContext);
    }
}
