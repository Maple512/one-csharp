namespace OneI;

public class Reporter
{
    private static readonly object _lock = new();

    private readonly AnsiConsole _console;

    private Reporter(AnsiConsole console)
    {
        _console = console;
    }

    public static Reporter Output { get; } = new Reporter(AnsiConsole.GetOutput());
    public static Reporter Error { get; } = new Reporter(AnsiConsole.GetOutput());
    public static Reporter Verbose { get; } = new Reporter(AnsiConsole.GetOutput());

    public void WriteLine(string message)
    {
        lock(_lock)
        {
            _console?.WriteLine(message);
        }
    }

    public void WriteLine()
    {
        lock(_lock)
        {
            _console?.Writer?.WriteLine();
        }
    }

    public void Write(string message)
    {
        lock(_lock)
        {
            _console?.Writer?.Write(message);
        }
    }

    public void WriteBanner(string content)
    {
        var border = new string('*', content.Length + 6);

        WriteLine($@"{border}
*  {content}  *
{border}".Green());
    }
}
