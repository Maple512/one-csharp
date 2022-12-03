namespace OneI.Logable;

public class Logger
{
    internal Logger() { }

    public void Write(
        LogLevel level,
        string message,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int? lineNumber = null)
    {

    }
}
