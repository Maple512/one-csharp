namespace OneI.Logable;

[StackTraceHidden]
[DebuggerStepThrough]
public static class InternalLog
{
    private static Action<string>? _writer;

    public static void Initialize(Action<string> writer) => _writer = writer;

    public static void Initialize(TextWriter writer) => _writer = (string message) =>
    {
        writer.WriteLine(message);

        writer.Flush();
    };

    public static void WriteLine(string message)
    {
        Debug.Assert(_writer != null);

        _writer?.Invoke(message);
    }
}
