namespace OneI.Logable.Templatizations.Tokenizations;

public class TextToken : ITemplateToken
{
    public TextToken(int index, string text)
    {
        Index = index;
        Text = text;
    }

    public int Index { get; }

    public string Text { get; }

    public override int GetHashCode() => HashCode.Combine(Index, Text);

    public override string ToString() => $"[Index: {Index}, Text: {Text}]";
}
