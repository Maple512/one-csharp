namespace OneI.Logable.Templatizations.Tokenizations;

using OneI.Logable.Rendering;

public class IndexerPropertyToken : ITemplatePropertyToken
{
    public IndexerPropertyToken(
        int parmeterIndex,
        PropertyTokenType type,
        string? format,
        int index,
        int? align,
        int? indent)
    {
        Type = type;
        Format = format;
        Index = index;
        Alignment = align.HasValue ? new TextAlignment(align.Value) : null;
        Indent = indent;
        ParameterIndex = parmeterIndex;
    }

    public int ParameterIndex { get; }

    public PropertyTokenType Type { get; }

    public string? Format { get; }

    public TextAlignment? Alignment { get; }

    public int? Indent { get; }

    public int Index { get; }

    public override int GetHashCode() => HashCode.Combine(ParameterIndex, Type, Format, Alignment, Indent, Index);

    public override string ToString() => $"[{nameof(Index)}: {Index}, {nameof(ParameterIndex)}: {ParameterIndex}]";
}
