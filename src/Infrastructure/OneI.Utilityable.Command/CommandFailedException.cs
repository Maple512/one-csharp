namespace OneI;

public class CommandFailedException : Exception
{
    public CommandFailedException(string message) : base(message) { }
}
