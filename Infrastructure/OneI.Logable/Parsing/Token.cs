namespace OneI.Logable.Parsing;

public abstract class Token
{
    public Token(string text, int position)
    {
        Text = text;
        Position = position;
    }

    public int Position { get; protected set; }

    public string Text { get; }

    public override string ToString() => Text;

    public override int GetHashCode() => Text.GetHashCode();
}
