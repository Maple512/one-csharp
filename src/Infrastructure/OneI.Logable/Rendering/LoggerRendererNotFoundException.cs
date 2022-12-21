namespace OneI.Logable.Rendering;

public class LoggerRendererNotFoundException : Exception
{
    public LoggerRendererNotFoundException()
        : base($"Not found any {nameof(ILoggerRenderer)}.")
    {
    }
}
