namespace OneI.Logable.Rendering;

using Formatters = LoggerConstants.Formatters;

internal static class RenderHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? Casing(string? value, string? format = null)
    {
        return format switch
        {
            Formatters.UpperCase => value?.ToUpperInvariant(),
            Formatters.LowerCase => value?.ToLowerInvariant(),
            _ => value,
        };
    }
}
