namespace OneI.Logable.Themes;

public class ConsoleTheme
{
    public ConsoleTheme()
    {
        _ = Spectre.Console.AnsiConsole.Create(new Spectre.Console.AnsiConsoleSettings
        {
            Ansi = Spectre.Console.AnsiSupport.Yes,
            ColorSystem = Spectre.Console.ColorSystemSupport.EightBit,
            Enrichment = new Spectre.Console.ProfileEnrichment(),
             
        });
    }
}
