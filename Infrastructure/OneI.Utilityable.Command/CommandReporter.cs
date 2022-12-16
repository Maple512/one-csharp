namespace OneI;

public static class CommandReporter
{
    private const string TimeSpanFormat = @"hh\:mm\:ss";
    private static readonly DateTime _initialTime = DateTime.Now;

    public static void BeginSection(string type, string name)
    {
        var message = $"{$"[{type,-10} >]".Green()}{$" [....] [{(DateTime.Now - _initialTime).ToString(TimeSpanFormat)}]".Blue()} {name}";

        Reporter.Output.WriteLine(message);
    }

    public static void SectionComment(string type, string comment)
    {
        Reporter.Output.WriteLine($"[{type,-10} -]".Green() + $" [....] [{(DateTime.Now - _initialTime).ToString(TimeSpanFormat)}]".Blue() + $" {comment}");
    }

    public static void EndSection(string type, string name, bool success)
    {
        var header = $"[{type,-10} <]";
        if(success)
        {
            header = header.Green();
        }
        else
        {
            header = header.Red();
        }

        var successString = success ? " OK " : "FAIL";

        Reporter.Output.WriteLine(header + $" [{successString}] [{(DateTime.Now - _initialTime).ToString(TimeSpanFormat)}]".Blue() + $" {name}");
    }
}
