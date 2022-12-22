namespace OneI;
/// <summary>
/// The command failed exception.
/// </summary>

public class CommandFailedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandFailedException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public CommandFailedException(string message) : base(message) { }
}
