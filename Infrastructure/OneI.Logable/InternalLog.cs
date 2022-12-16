namespace OneI.Logable;

[DebuggerStepThrough]
public static class InternalLog
{
    private static Action<string>? _writer;

    public static Action<string> Output
    {
        get
        {
            if(_writer == null)
            {
                return static msg => Debug.WriteLine(msg);
            }

            return _writer;
        }
    }

    public static void Initialize(Action<string> writer)
    {
        _writer = writer;
    }

    public static void Initialize(TextWriter writer)
    {
        _writer = (string message) =>
        {
            writer.WriteLine(message);

            writer.Flush();
        };
    }

    public static void WriteLine(string message)
    {
        Output.Invoke(message);
    }
}
