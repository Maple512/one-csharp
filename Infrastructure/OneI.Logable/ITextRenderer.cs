namespace OneI.Logable;

public interface ITextRenderer
{
    void Render(LoggerContext context, TextWriter writer);
}
