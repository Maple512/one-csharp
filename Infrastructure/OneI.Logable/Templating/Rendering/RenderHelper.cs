namespace OneI.Logable.Templating.Rendering;

using Formatters = OneI.Logable.LoggerConstants.Formatters;

public static class RenderHelper
{
    /// <summary>
    /// 向指定的方向填充指定长度的空格（' '）
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <param name="alignment">指定填充方向和长度</param>
    public static void Padding(TextWriter writer, string? value, Alignment? alignment)
    {
        if(alignment.HasValue is false
            || value is null
            || value.Length >= alignment.Value.Width)
        {
            writer.Write(value);

            return;
        }

        var align = alignment.Value;

        var pad = align.Width - value.Length;

        if(align.Direction == Direction.Left)
        {
            writer.Write(value);
        }

        writer.Write(new string(' ', pad));

        if(align.Direction == Direction.Right)
        {
            writer.Write(value);
        }
    }

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
