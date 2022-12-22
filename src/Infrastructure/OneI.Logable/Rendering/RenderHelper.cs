namespace OneI.Logable.Rendering;

using Formatters = LoggerConstants.Formatters;
/// <summary>
/// The render helper.
/// </summary>

public static class RenderHelper
{
    /// <summary>
    /// 转换大小写
    /// </summary>
    /// <param name="value"></param>
    /// <param name="format"><code>l -> to lower-case</code><code>u -> to upper-case</code></param>
    /// <returns></returns>
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
