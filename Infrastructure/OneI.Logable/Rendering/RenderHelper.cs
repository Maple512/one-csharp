namespace OneI.Logable.Rendering;

using Formatters = LoggerConstants.Formatters;

public static class RenderHelper
{
    public static void Padding(TextWriter writer, string? value, Alignment? alignment, int? indent = null)
    {
        if(value.IsNullOrWhiteSpace())
        {
            writer.Write(value);
            return;
        }

        if(indent != null)
        {
            writer.Write(new string(' ', indent.Value));
        }

        if(alignment.HasValue is false
            || value.Length >= alignment.Value.Width)
        {
            writer.Write(value);

            return;
        }

        var pad = alignment.Value.Width - value.Length;

        if(alignment.Value.Direction == Direction.Left)
        {
            writer.Write(value);
        }

        writer.Write(new string(' ', pad));

        if(alignment.Value.Direction == Direction.Right)
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
