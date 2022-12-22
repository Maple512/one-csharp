namespace OneI;
/// <summary>
/// The command reporter.
/// </summary>

public static class CommandReporter
{
    /// <summary>
    /// The time span format.
    /// </summary>
    private const string TimeSpanFormat = @"hh\:mm\:ss";
    private static readonly DateTime _initialTime = DateTime.Now;

    /// <summary>
    /// Begins the section.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="name">The name.</param>
    public static void BeginSection(string type, string name)
    {
        var message = $"{$"[{type,-10} >]".Green()}{$" [....] [{(DateTime.Now - _initialTime).ToString(TimeSpanFormat)}]".Blue()} {name}";

        Reporter.Output.WriteLine(message);
    }

    /// <summary>
    /// Sections the comment.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="comment">The comment.</param>
    public static void SectionComment(string type, string comment)
    {
        Reporter.Output.WriteLine($"[{type,-10} -]".Green() + $" [....] [{(DateTime.Now - _initialTime).ToString(TimeSpanFormat)}]".Blue() + $" {comment}");
    }

    /// <summary>
    /// Ends the section.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="name">The name.</param>
    /// <param name="success">If true, success.</param>
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
