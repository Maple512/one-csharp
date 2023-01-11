namespace OneI.Logable.Formatting;

using OneI.Logable.Rendering;
using OneI.Logable;

/// <summary>
/// The level formatter.
/// </summary>
public class LevelFormatter : IFormatter<LogLevel>
{
    public static readonly IFormatter<LogLevel> Instance = new LevelFormatter();

    /// <summary>
    /// Formats the.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="writer">The writer.</param>
    /// <param name="format">The format.</param>
    /// <param name="formatProvider">The format provider.</param>
    public virtual void Format(LogLevel value, TextWriter writer, string? format = null, IFormatProvider? formatProvider = null)
    {
        writer.Write(LevelFormatHelper.Format(value, format));
    }
}

/// <summary>
/// The level format helper.
/// </summary>
internal static class LevelFormatHelper
{
    private static readonly string[][] _levelMap =
        {
            new []{ "V", "Vrb", "Verbose" },
            new []{ "D", "Dbg", "Debug" },
            new []{ "I", "Inf", "Information" },
            new []{ "W", "Wrn", "Warning" },
            new []{ "E", "Err", "Error" },
            new []{ "F", "Ftl", "Fatal" }
        };

    /// <summary>
    /// Formats the.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <param name="format">The format.</param>
    /// <returns>A string.</returns>
    public static string Format(LogLevel level, string? format = null)
    {
        if(format.IsNullOrWhiteSpace()
            || level < LogLevel.Verbose && level > LogLevel.Fatal)
        {
            return RenderHelper.Casing(level.ToString(), format)!;
        }

        int? order = null;
        string? @case = null;
        for(var i = 0; i < Math.Min(format!.Length, 2); i++)
        {
            var item = format[i];
            if(char.IsDigit(item))
            {
                order = Math.Abs(item - '0');
            }
            else if(char.IsLetter(item))
            {
                @case = char.ToString(item);
            }
        }

        var minikers = _levelMap[(int)level];

        var index = Math.Min(order ?? 0, minikers.Length) - 1;

        if(index < 0)
        {
            index = 2;
        }

        var moniker = minikers[index];

        return RenderHelper.Casing(moniker, @case)!;
    }
}
