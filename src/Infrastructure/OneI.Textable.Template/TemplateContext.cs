namespace OneI.Textable;

using OneI.Textable.Templating;
/// <summary>
/// The template context.
/// </summary>

public class TemplateContext
{
    private readonly Token[] _tokens;
    private readonly PropertyToken[] _properties;

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateContext"/> class.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="tokens">The tokens.</param>
    public TemplateContext(string text, List<Token> tokens)
    {
        Text = text;
        _tokens = tokens.ToArray();

        _properties = tokens.OfType<PropertyToken>().ToArray();
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the tokens.
    /// </summary>
    public IReadOnlyList<Token> Tokens => _tokens;

    /// <summary>
    /// Gets the properties.
    /// </summary>
    public IReadOnlyList<PropertyToken> Properties => _properties;

    /// <summary>
    /// Tos the string.
    /// </summary>
    /// <returns>A string.</returns>
    public override string ToString()
    {
        return Text;
    }
}
