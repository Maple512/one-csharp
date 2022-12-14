namespace OneI.Logable.Templating;

public class TextToken : Token
{
    public TextToken(int position, string text) : base(position, text)
    {
    }

    public TextToken(int position, ref ReadOnlySpan<char> text) : base(position, ref text)
    {
    }
}
