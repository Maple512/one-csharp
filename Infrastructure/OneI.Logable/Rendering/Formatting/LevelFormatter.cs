namespace OneI.Logable.Rendering.Formatting;

using OneI.Logable.Rendering;

public static class LevelFormatter
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
