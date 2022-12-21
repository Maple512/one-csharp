namespace OneI.Textable;

using OneI.Textable.Templating;

public class TemplateContext
{
    private readonly Token[] _tokens;
    private readonly PropertyToken[] _properties;

    public TemplateContext(string text, List<Token> tokens)
    {
        Text = text;
        _tokens = tokens.ToArray();

        _properties = tokens.OfType<PropertyToken>().ToArray();
    }

    public string Text { get; }

    public IReadOnlyList<Token> Tokens => _tokens;

    public IReadOnlyList<PropertyToken> Properties => _properties;

    public override string ToString()
    {
        return Text;
    }
}
