namespace OneI.Textable.Templating;

/// <summary>
/// The text token.
/// </summary>
public class TextToken : Token
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextToken"/> class.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="text">The text.</param>
    public TextToken(int position, string text) : base(position, text)
    {
    }
}
