namespace OneI.Logable;
/// <summary>
/// The internal log.
/// </summary>

[DebuggerStepThrough]
public static class InternalLog
{
    private static Action<string>? _writer;

    /// <summary>
    /// Gets the output.
    /// </summary>
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

    /// <summary>
    /// Initializes the.
    /// </summary>
    /// <param name="writer">The writer.</param>
    public static void Initialize(Action<string> writer)
    {
        _writer = writer;
    }

    /// <summary>
    /// Initializes the.
    /// </summary>
    /// <param name="writer">The writer.</param>
    public static void Initialize(TextWriter writer)
    {
        _writer = (string message) =>
        {
            writer.WriteLine(message);

            writer.Flush();
        };
    }

    /// <summary>
    /// Writes the line.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void WriteLine(string message)
    {
        Output.Invoke(message);
    }
}
