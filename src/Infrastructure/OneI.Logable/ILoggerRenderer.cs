namespace OneI.Logable;

public interface ILoggerRenderer
{
    void Render(LoggerContext context, TextWriter writer);
}
