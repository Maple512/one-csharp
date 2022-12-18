namespace OneI.Logable;

using OneI.Logable.Rendering;

public interface ITextRendererProvider
{
    ILoggerRenderer GetTextRenderer(in LoggerContext context);
}
