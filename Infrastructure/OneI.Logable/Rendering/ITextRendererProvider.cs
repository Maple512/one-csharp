namespace OneI.Logable;

public interface ITextRendererProvider
{
    ITextRenderer? GetTextRenderer(in LoggerContext context);
}
