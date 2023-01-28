namespace OneI.Logable.Templatizations.Tokenizations;

public class NamedPropertyToken : ITemplatePropertyToken
{
    internal NamedPropertyToken(
        string name,
        PropertyTokenType type,
        string? format,
        int? align,
        int? indent)
    {
        Name = name;
        Type = type;
        Format = format;
        Alignment = align.HasValue ? new TextAlignment(align.Value) : null;
        Indent = indent;
    }

    public string Name { get; }

    public PropertyTokenType Type { get; }

    public string? Format { get; }

    public TextAlignment? Alignment { get; }

    public int? Indent { get; }

    public override int GetHashCode() => HashCode.Combine(Name, Type, Format, Alignment, Indent);

    public override string ToString() => $"[{nameof(Name)}: {Name}]";
}
