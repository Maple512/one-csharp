namespace OneI.Textable.Templating;

/// <summary>
/// The token.
/// </summary>
public class Token
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Token"/> class.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="text">The text.</param>
    public Token(int position, string text)
    {
        Position = position;
        Text = text;
    }

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    public int Position { get; protected set; }

    /// <summary>
    /// Gets the text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Tos the string.
    /// </summary>
    /// <returns>A string.</returns>
    public override string ToString()
    {
        return Text;
    }
}
