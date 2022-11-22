namespace OneI.Logable.Parsing;

using System.Collections.Generic;

public class TextParserResult
{
    public TextParserResult(IEnumerable<Token> tokens)
    {
        Tokens = tokens;
    }

    public IEnumerable<Token> Tokens { get; }
}
