namespace OneI.Textable.Templating;

using System.Globalization;
using OneI.Textable.Rendering;

public class PropertyToken : Token
{
    public PropertyToken(
        string name,
        string text,
        int index,
        int position,
        string? format,
        Alignment? alignment = null,
        int? indent = null) : base(position, text)
    {
        Name = name;
        Format = format;
        Index = index;
        Alignment = alignment;
        Indent = indent;

        if(int.TryParse(name, NumberStyles.None, CultureInfo.InvariantCulture, out var parameterIndex) &&
           parameterIndex >= 0)
        {
            ParameterIndex = parameterIndex;
        }
    }

    public string Name { get; }

    public string? Format { get; }

    public int Index { get; }

    public Alignment? Alignment { get; }

    public int? Indent { get; }

    public int? ParameterIndex { get; }

    internal void ResetPosition(int position)
    {
        Position = position;
    }
}
