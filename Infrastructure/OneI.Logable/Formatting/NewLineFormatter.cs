namespace OneI.Logable.Formatting;

using System;
using OneI.Textable;

public class NewLineFormatter : IFormatter<string?>
{
    public void Format(string? value, TextWriter wtier, string? format = null, IFormatProvider? formatProvider = null)
    {
        wtier.Write(Environment.NewLine);
    }
}
