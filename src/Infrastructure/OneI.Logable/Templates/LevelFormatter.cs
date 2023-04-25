namespace OneI.Logable.Templates;

using OneI.Generateable.CodeGenerated;
using static LoggerConstants;

public static class LevelFormatter
{
    private static readonly string[][] _levelMap =
    {
        new[] { "V", "Vrb", "Verbose", },
        new[] { "D", "Dbg", "Debug", },
        new[] { "I", "Inf", "Information", } ,
        new[] { "W", "Wrn", "Warning", },
        new[] { "E", "Err", "Error", },
        new[] { "F", "Ftl", "Fatal", },
    };

    public static string Format(LogLevel level, ReadOnlySpan<char> format)
    {
        var @case = char.MinValue;
        if(level is < LogLevel.Verbose or > LogLevel.Fatal)
        {
            if(format.Length > 0)
            {
                @case = format[0];
            }

            return Casing(level.ToFastString(), @case);
        }

        var order = 0;

        for(var i = 0; i < Math.Min(format.Length, 2); i++)
        {
            ref readonly var item = ref format[i];
            if(char.IsDigit(item))
            {
                order = item - '0';
            }
            else if(char.IsLetter(item))
            {
                @case = item;
            }
        }

        var minikers = _levelMap[(int)level];

        var index = Math.Min(order, minikers.Length) - 1;

        if(index < 0)
        {
            index = 2;
        }

        var moniker = minikers[index];

        return Casing(moniker, @case)!;
    }

    [return: NotNullIfNotNull("value")]
    public static string? Casing(string? value, char format)
        => format switch
        {
            Formatters.UpperCase => value?.ToUpperInvariant(),
            Formatters.LowerCase => value?.ToLowerInvariant(),
            _ => value,
        };
}
