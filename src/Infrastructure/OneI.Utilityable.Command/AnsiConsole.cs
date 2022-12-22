namespace OneI;
/// <summary>
/// The ansi console.
/// </summary>

public class AnsiConsole
{
    /// <summary>
    /// Prevents a default instance of the <see cref="AnsiConsole"/> class from being created.
    /// </summary>
    /// <param name="writer">The writer.</param>
    private AnsiConsole(TextWriter writer)
    {
        Writer = writer;

        OriginalForegroundColor = Console.ForegroundColor;
    }

    private int _boldRecursion;

    /// <summary>
    /// Gets the output.
    /// </summary>
    /// <returns>An AnsiConsole.</returns>
    public static AnsiConsole GetOutput()
    {
        return new AnsiConsole(Console.Out);
    }

    /// <summary>
    /// Gets the error.
    /// </summary>
    /// <returns>An AnsiConsole.</returns>
    public static AnsiConsole GetError()
    {
        return new AnsiConsole(Console.Error);
    }

    /// <summary>
    /// Gets the writer.
    /// </summary>
    public TextWriter Writer { get; }

    /// <summary>
    /// Gets the original foreground color.
    /// </summary>
    public ConsoleColor OriginalForegroundColor { get; }

    /// <summary>
    /// Sets the color.
    /// </summary>
    /// <param name="color">The color.</param>
    private void SetColor(ConsoleColor color)
    {
        const int Light = 0x08;
        var c = (int)color;

        Console.ForegroundColor =
            c < 0 ? color :                                   // unknown, just use it
            _boldRecursion > 0 ? (ConsoleColor)(c | Light) :  // ensure color is light
            (ConsoleColor)(c & ~Light);                       // ensure color is dark
    }

    /// <summary>
    /// Sets the bold.
    /// </summary>
    /// <param name="bold">If true, bold.</param>
    private void SetBold(bool bold)
    {
        _boldRecursion += bold ? 1 : -1;
        if(_boldRecursion > 1 || _boldRecursion == 1 && !bold)
        {
            return;
        }

        // switches on _boldRecursion to handle boldness
        SetColor(Console.ForegroundColor);
    }

    /// <summary>
    /// Writes the line.
    /// </summary>
    /// <param name="message">The message.</param>
    public void WriteLine(string message)
    {
        Write(message);
        Writer.WriteLine();
    }


    /// <summary>
    /// Writes the.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Write(string message)
    {
        var escapeScan = 0;
        for(; ; )
        {
            var escapeIndex = message.IndexOf("\x1b[", escapeScan, StringComparison.Ordinal);
            if(escapeIndex == -1)
            {
                var text = message[escapeScan..];
                Writer.Write(text);
                break;
            }
            else
            {
                var startIndex = escapeIndex + 2;
                var endIndex = startIndex;
                while(endIndex != message.Length &&
                    message[endIndex] >= 0x20 &&
                    message[endIndex] <= 0x3f)
                {
                    endIndex += 1;
                }

                var text = message[escapeScan..escapeIndex];
                Writer.Write(text);
                if(endIndex == message.Length)
                {
                    break;
                }

                switch(message[endIndex])
                {
                    case 'm':
                        int value;
                        if(int.TryParse(message[startIndex..endIndex], out value))
                        {
                            switch(value)
                            {
                                case 1:
                                    SetBold(true);
                                    break;
                                case 22:
                                    SetBold(false);
                                    break;
                                case 30:
                                    SetColor(ConsoleColor.Black);
                                    break;
                                case 31:
                                    SetColor(ConsoleColor.Red);
                                    break;
                                case 32:
                                    SetColor(ConsoleColor.Green);
                                    break;
                                case 33:
                                    SetColor(ConsoleColor.Yellow);
                                    break;
                                case 34:
                                    SetColor(ConsoleColor.Blue);
                                    break;
                                case 35:
                                    SetColor(ConsoleColor.Magenta);
                                    break;
                                case 36:
                                    SetColor(ConsoleColor.Cyan);
                                    break;
                                case 37:
                                    SetColor(ConsoleColor.Gray);
                                    break;
                                case 39:
                                    Console.ForegroundColor = OriginalForegroundColor;
                                    break;
                            }
                        }

                        break;
                }

                escapeScan = endIndex + 1;
            }
        }
    }
}
