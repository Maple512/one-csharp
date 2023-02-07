namespace OneI;

/// <summary>
/// 参数转义
/// source: https://github.com/dotnet/runtime/blob/main/src/installer/tests/TestUtils
/// </summary>
public static class ArgumentEscaper
{
    /// <summary>
    /// Undo the processing which took place to create string[] args in Main,
    /// so that the next process will receive the same string[] args
    ///
    /// See here for more info:
    /// https://docs.microsoft.com/en-us/archive/blogs/twistylittlepassagesallalike/everyone-quotes-command-line-arguments-the-wrong-way
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string EscapeAndConcatenateArgArrayForProcessStart(IEnumerable<string> args)
    {
        return string.Join(" ", EscapeArgArray(args));
    }

    /// <summary>
    /// Undo the processing which took place to create string[] args in Main,
    /// so that the next process will receive the same string[] args
    ///
    /// See here for more info:
    /// https://docs.microsoft.com/en-us/archive/blogs/twistylittlepassagesallalike/everyone-quotes-command-line-arguments-the-wrong-way
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private static IEnumerable<string> EscapeArgArray(IEnumerable<string> args)
    {
        var escapedArgs = new List<string>();

        foreach(var arg in args)
        {
            escapedArgs.Add(EscapeArg(arg));
        }

        return escapedArgs;
    }

    /// <summary>
    /// Escapes the arg.
    /// </summary>
    /// <param name="arg">The arg.</param>
    /// <returns>A string.</returns>
    private static string EscapeArg(string arg)
    {
        var sb = new StringBuilder();

        var quoted = ShouldSurroundWithQuotes(arg);
        if(quoted)
        {
            _ = sb.Append('"');
        }

        for(var i = 0; i < arg.Length; ++i)
        {
            var backslashCount = 0;

            // Consume All Backslashes
            while(i < arg.Length && arg[i] == '\\')
            {
                backslashCount++;
                i++;
            }

            // Escape any backslashes at the end of the arg
            // This ensures the outside quote is interpreted as
            // an argument delimiter
            if(i == arg.Length)
            {
                _ = sb.Append('\\', 2 * backslashCount);
            }

            // Escape any preceding backslashes and the quote
            else if(arg[i] == '"')
            {
                _ = sb.Append('\\', 2 * backslashCount + 1);
                _ = sb.Append('"');
            }

            // Output any consumed backslashes and the character
            else
            {
                _ = sb.Append('\\', backslashCount);
                _ = sb.Append(arg[i]);
            }
        }

        if(quoted)
        {
            _ = sb.Append('"');
        }

        return sb.ToString();
    }

    /// <summary>
    /// Prepare as single argument to
    /// roundtrip properly through cmd.
    ///
    /// This prefixes every character with the '^' character to force cmd to
    /// interpret the argument string literally. An alternative option would
    /// be to do this only for cmd metacharacters.
    ///
    /// See here for more info:
    /// https://docs.microsoft.com/en-us/archive/blogs/twistylittlepassagesallalike/everyone-quotes-command-line-arguments-the-wrong-way
    /// </summary>
    /// <returns></returns>
    internal static bool ShouldSurroundWithQuotes(string argument)
    {
        // Don't quote already quoted strings
        if(argument.StartsWith("\"", StringComparison.Ordinal) &&
                argument.EndsWith("\"", StringComparison.Ordinal))
        {
            return false;
        }

        // Only quote if whitespace exists in the string
        if(argument.Contains(' ') || argument.Contains('\t') || argument.Contains('\n'))
        {
            return true;
        }

        return false;
    }
}
