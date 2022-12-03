namespace OneI.Logable.Templating;

public class Token
{
    public Token(int position, string text)
    {
        Position = position;
        Text = text;
    }

    public int Position { get; protected set; }

    public string Text { get; }

    public override string ToString() => Text;
}
