namespace OneI.Logable.Formatting;

using System;
using System.Collections.Generic;
using System.IO;
using OneI.Logable.Parsing;

public class LoggerFormatter : ILoggerFormatter
{
    private readonly IFormatProvider? _formatProvider;
    private readonly IEnumerable<ITextToken> _tokens;

    public LoggerFormatter(ReadOnlySpan<byte> text, IFormatProvider? formatProvider = null)
    {
        _tokens = TextParser.Parse(text);

        _formatProvider = formatProvider;
    }

    public void Format(ILoggerContext context, TextWriter output)
    {
        foreach(var token in _tokens)
        {
            if(token is TextToken textToken)
            {
                output.Write(textToken.ToString());

                continue;
            }
        }
    }
}
