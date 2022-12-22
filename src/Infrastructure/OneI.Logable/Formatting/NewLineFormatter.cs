namespace OneI.Logable.Formatting;

using System;
using OneI.Textable;
/// <summary>
/// The new line formatter.
/// </summary>

public class NewLineFormatter : IFormatter<string?>
{
    /// <summary>
    /// Formats the.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="wtier">The wtier.</param>
    /// <param name="format">The format.</param>
    /// <param name="formatProvider">The format provider.</param>
    public void Format(string? value, TextWriter wtier, string? format = null, IFormatProvider? formatProvider = null)
    {
        wtier.Write(Environment.NewLine);
    }
}
