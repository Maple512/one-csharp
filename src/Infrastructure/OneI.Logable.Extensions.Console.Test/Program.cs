namespace OneI.Logable;

using Spectre.Console;

public class Program
{
    public static int Main(string[] args)
    {
        if(AnsiConsole.Profile.Capabilities.Links)
        {
            AnsiConsole.MarkupLine("[link=https://patriksvensson.se]Click to visit my blog[/]!");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]It looks like your terminal doesn't support links[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[yellow](╯°□°)╯[/]︵ [blue]┻━┻[/]");
        }

        try
        {
            throw new Exception("exception message");
        }
        catch(Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            AnsiConsole.WriteException(ex, ExceptionFormats.ShowLinks);
        }

        return 0;
    }
}
