namespace OneI;
/// <summary>
/// The ansi color extensions.
/// </summary>

public static class AnsiColorExtensions
{
    /// <summary>
    /// Blacks the.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>A string.</returns>
    public static string Black(this string text)
    {
        return "\x1B[30m" + text + "\x1B[39m";
    }

    /// <summary>
    /// Reds the.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>A string.</returns>
    public static string Red(this string text)
    {
        return "\x1B[31m" + text + "\x1B[39m";
    }
    /// <summary>
    /// Greens the.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>A string.</returns>
    public static string Green(this string text)
    {
        return "\x1B[32m" + text + "\x1B[39m";
    }

    /// <summary>
    /// Yellows the.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>A string.</returns>
    public static string Yellow(this string text)
    {
        return "\x1B[33m" + text + "\x1B[39m";
    }

    /// <summary>
    /// Blues the.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>A string.</returns>
    public static string Blue(this string text)
    {
        return "\x1B[34m" + text + "\x1B[39m";
    }

    /// <summary>
    /// Magentas the.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>A string.</returns>
    public static string Magenta(this string text)
    {
        return "\x1B[35m" + text + "\x1B[39m";
    }

    /// <summary>
    /// Cyans the.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>A string.</returns>
    public static string Cyan(this string text)
    {
        return "\x1B[36m" + text + "\x1B[39m";
    }

    /// <summary>
    /// Whites the.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>A string.</returns>
    public static string White(this string text)
    {
        return "\x1B[37m" + text + "\x1B[39m";
    }

    /// <summary>
    /// Bolds the.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>A string.</returns>
    public static string Bold(this string text)
    {
        return "\x1B[1m" + text + "\x1B[22m";
    }
}
