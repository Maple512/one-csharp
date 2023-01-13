namespace OneI.Logable.Rendering;

public interface ILoggerRendererSelector
{
    ILoggerRenderer GetRenderer(LoggerContext context);
}
