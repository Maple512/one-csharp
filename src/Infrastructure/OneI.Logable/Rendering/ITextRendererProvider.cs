namespace OneI.Logable;

using OneI.Logable.Rendering;
/// <summary>
/// The text renderer provider.
/// </summary>

public interface ITextRendererProvider
{
    /// <summary>
    /// Gets the text renderer.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>An ILoggerRenderer.</returns>
    ILoggerRenderer GetTextRenderer(in LoggerContext context);
}
