namespace OneI.Logable.Templating;

using System.Collections.Generic;

public class TextTemplate
{
    private readonly Token[] _tokens;

    public TextTemplate(string text, IEnumerable<Token> tokens)
    {
        Text = text;
        _tokens = tokens.ToArray();
    }

    public string Text { get; }

    public IReadOnlyList<Token> Tokens => _tokens;
}
