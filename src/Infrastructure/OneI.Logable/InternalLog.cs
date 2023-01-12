namespace OneI.Logable;

[DebuggerStepThrough]
public static class InternalLog
{
    private static Action<string>? _writer;

    public static void Initialize(Action<string> writer)
    {
        _writer = writer;
    }

    public static void Initialize(TextWriter writer)
    {
        _writer = (string message) =>
        {
            writer.Write(message);

            writer.Flush();
        };
    }

    public static void Write(in string message)
    {
        _writer?.Invoke(message);
    }

    public static void WriteLine()
    {
        _writer?.Invoke(Environment.NewLine);
    }

    public static void WriteLine(in string message)
    {
        _writer?.Invoke(message);
        _writer?.Invoke(Environment.NewLine);
    }
}
