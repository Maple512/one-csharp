namespace OneI.Logable;

public interface ITextToken
{
    int Position { get; }

    string Text { get; }

    public virtual int Length => Text.Length;
}
