namespace OneI.Logable.Templatizations.Tokenizations;

public class NamedPropertyToken : ITemplatePropertyToken
{
    internal NamedPropertyToken(
        string name,
        PropertyTokenType type,
        string? format,
        int index,
        int? align,
        int? indent)
    {
        Name = name;
        Type = type;
        Format = format;
        Index = index;
        Alignment = align.HasValue ? new TextAlignment(align.Value) : null;
        Indent = indent;
    }

    public string Name { get; }
    public PropertyTokenType Type { get; }
    public string? Format { get; }
    public int Index { get; private set; }
    public TextAlignment? Alignment { get; }
    public int? Indent { get; }

    public override int GetHashCode() => HashCode.Combine(Name, Type, Format, Index, Alignment, Indent, Index);

    public void ResetIndex(int index)
    {
        Index = index;
    }

    public override string ToString() => $"[{nameof(Index)}: {Index}, {nameof(Name)}: {Name}]";
}
