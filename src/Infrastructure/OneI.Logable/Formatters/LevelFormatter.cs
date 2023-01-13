namespace OneI.Logable.Formatters;

using OneI.Logable;
using OneI.Logable.Templatizations;

using static OneI.Logable.LoggerConstants;

public class LevelFormatter : IPropertyValueFormatter<LogLevel>
{
    public static readonly IPropertyValueFormatter<LogLevel> Instance = new LevelFormatter();

    public virtual void Format(in LogLevel value, TextWriter writer, string? format = null, IFormatProvider? formatProvider = null)
    {
        writer.Write(LevelFormatHelper.Format(value, format));
    }
}

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

    public static string Format(LogLevel level, string? format = null)
    {
        if(format.IsNullOrWhiteSpace()
            || level < LogLevel.Verbose && level > LogLevel.Fatal)
        {
            return Casing(level.ToString(), format)!;
        }

        int? order = null;
        string? @case = null;
        for(var i = 0; i < Math.Min(format.Length, 2); i++)
        {
            var item = format[i];
            if(char.IsDigit(item))
            {
                order = item - '0';
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

        return Casing(moniker, @case)!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? Casing(in string? value, in string? format = null)
    {
        return format switch
        {
            Formatters.UpperCase => value?.ToUpperInvariant(),
            Formatters.LowerCase => value?.ToLowerInvariant(),
            _ => value,
        };
    }
}
