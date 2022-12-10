namespace OneI.Logable.Templating;

using System.Collections.Generic;

public class TextTemplate
{
    private readonly Token[] _tokens;
    private readonly PropertyToken[] _properties;

    public TextTemplate(string text, IEnumerable<Token> tokens)
    {
        Text = text;
        _tokens = tokens.ToArray();

        _properties = tokens.OfType<PropertyToken>().ToArray();
    }

    public string Text { get; }

    public IReadOnlyList<Token> Tokens => _tokens;

    public IReadOnlyList<PropertyToken> Properties => _properties;

    public bool TryGetPropertyToken(int index, out PropertyToken? token)
    {
        var propertyTokens = Tokens.OfType<PropertyToken>();

        token = propertyTokens.FirstOrDefault(x => x.ParameterIndex == index);

        if(token is null
            && propertyTokens.Count() > (index + 1))
        {
            token = propertyTokens.ElementAt(index);
        }

        return token is not null;
    }
}
