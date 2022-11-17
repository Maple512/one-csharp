namespace OneI.Logable.Parsing;

using System.Collections.Generic;

public class TextParserResult
{
    public TextParserResult(IEnumerable<TextToken> tokens)
    {
        Tokens = tokens;
    }

    public IEnumerable<TextToken> Tokens { get; }
}
