namespace OneI.Logable.Templates;

public class IndexerProperty : ITemplateProperty
{
    public IndexerProperty(int index, ITemplatePropertyValue value)
    {
        Index = index;
        Value = value;
    }

    public int Index { get; }

    public ITemplatePropertyValue Value { get; }

    public override int GetHashCode() => HashCode.Combine(Index, Value);

    public override string ToString() => $"[Index: {Index}, Value: {Value}]";
}
