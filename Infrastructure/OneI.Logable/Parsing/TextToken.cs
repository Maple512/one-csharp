namespace OneI.Logable.Parsing;

public class TextToken : ITextToken
{
    public static TextToken Empty { get; } = new TextToken(string.Empty, -1);

    public TextToken(string text, int position)
    {
        Text = text;
        Position = position;
    }

    public int Position { get; protected set; }

    public string Text { get; }

    public override string ToString() => Text;

    public override int GetHashCode() => Text.GetHashCode();
}
