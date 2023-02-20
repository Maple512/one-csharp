namespace OneI.Logable;

using OneI.Logable.Templates;

public static class LoggerContextExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteTo(this LoggerContext context, TextWriter writer, IFormatProvider? formatProvider = null)
    {
        TemplateRenderHelper.Render(writer, context, formatProvider);
    }
}
