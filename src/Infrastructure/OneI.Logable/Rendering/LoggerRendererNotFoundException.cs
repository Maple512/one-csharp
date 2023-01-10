namespace OneI.Logable.Rendering;
/// <summary>
/// The logger renderer not found exception.
/// </summary>

public class LoggerRendererNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerRendererNotFoundException"/> class.
    /// </summary>
    public LoggerRendererNotFoundException()
        : base($"Not found any {nameof(ILoggerRenderer)}.")
    {
    }
}
