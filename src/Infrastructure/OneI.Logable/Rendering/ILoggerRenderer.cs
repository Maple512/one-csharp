namespace OneI.Logable.Rendering;

public interface ILoggerRenderer
{
    void Render(LoggerContext context, TextWriter writer);
}
