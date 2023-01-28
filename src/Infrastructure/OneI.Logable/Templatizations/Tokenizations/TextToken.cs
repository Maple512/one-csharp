namespace OneI.Logable.Templatizations.Tokenizations;

public class TextToken : ITemplateToken
{
    public TextToken(string text)
    {
        Text = text;
    }

    public string Text { get; }

    public override int GetHashCode() => HashCode.Combine(Text);

    public override string ToString() => $"[Text: {Text}]";
}
