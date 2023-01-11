namespace OneI.Logable.Templating;

using OneI.Logable.Rendering;

public class PropertyToken : Token
{
    public const int NameLengthLimit = 32;
    public const int FormatLengthLimit = 32;
    public const int IndexLengthLimit = 3;
    public const int IndexNumericLimit = 99;
    public const int AlignLengthLimit = 3;
    public const int AlignNumericLimit = 99;

    public PropertyToken(
        string name,
        string text,
        int index,
        int position,
        string? format,
        Alignment? alignment = null,
        int? indent = null,
        int? parameterIndex = null) : base(position, text)
    {
        Name = name;
        Format = format;
        Index = index;
        Alignment = alignment;
        Indent = indent;
        ParameterIndex = parameterIndex;
    }

    public PropertyToken(
        ref ReadOnlySpan<char> name,
        ref ReadOnlySpan<char> text,
        int index,
        int position,
        ref ReadOnlySpan<char> format,
        int? alignment = null,
        int? indent = null,
        int? parameterIndex = null) : base(position, ref text)
    {
        Name = name.ToString();
        Format = format.IsEmpty ? null : format.ToString();
        Index = index;
        Alignment = alignment.HasValue ? new Alignment(alignment.Value) : null;
        Indent = indent;
        ParameterIndex = parameterIndex;
    }

    public string Name { get; }

    public string? Format { get; }

    public int Index { get; }

    public Alignment? Alignment { get; }

    public int? Indent { get; }

    public int? ParameterIndex { get; }

    [Obsolete]
    internal void ResetPosition(int position)
    {
        Position = position;
    }
}
