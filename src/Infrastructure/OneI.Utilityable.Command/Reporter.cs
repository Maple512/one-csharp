namespace OneI;
/// <summary>
/// The reporter.
/// </summary>

public class Reporter
{
    private static readonly object _lock = new();

    private readonly AnsiConsole _console;

    /// <summary>
    /// Prevents a default instance of the <see cref="Reporter"/> class from being created.
    /// </summary>
    /// <param name="console">The console.</param>
    private Reporter(AnsiConsole console)
    {
        _console = console;
    }

    /// <summary>
    /// Gets the output.
    /// </summary>
    public static Reporter Output { get; } = new Reporter(AnsiConsole.GetOutput());
    /// <summary>
    /// Gets the error.
    /// </summary>
    public static Reporter Error { get; } = new Reporter(AnsiConsole.GetOutput());
    /// <summary>
    /// Gets the verbose.
    /// </summary>
    public static Reporter Verbose { get; } = new Reporter(AnsiConsole.GetOutput());

    /// <summary>
    /// Writes the line.
    /// </summary>
    /// <param name="message">The message.</param>
    public void WriteLine(string message)
    {
        lock(_lock)
        {
            _console?.WriteLine(message);
        }
    }

    /// <summary>
    /// Writes the line.
    /// </summary>
    public void WriteLine()
    {
        lock(_lock)
        {
            _console?.Writer?.WriteLine();
        }
    }

    /// <summary>
    /// Writes the.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Write(string message)
    {
        lock(_lock)
        {
            _console?.Writer?.Write(message);
        }
    }

    /// <summary>
    /// Writes the banner.
    /// </summary>
    /// <param name="content">The content.</param>
    public void WriteBanner(string content)
    {
        var border = new string('*', content.Length + 6);

        WriteLine($@"{border}
*  {content}  *
{border}".Green());
    }
}
