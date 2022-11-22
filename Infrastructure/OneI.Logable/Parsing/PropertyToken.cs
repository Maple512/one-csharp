namespace OneI.Logable.Parsing;

using System.Globalization;
using OneI.Logable.Rendering;

public class PropertyToken : Token
{
    public PropertyToken(
        string name,
        int index,
        string text,
        int position = -1,
        string? format = null,
        Alignment? alignment = null,
        PropertyTokenType parsingType = PropertyTokenType.Stringify) : base(text, position)
    {
        Name = CheckTools.NotNullOrWhiteSpace(name);
        ParameterIndex = Index = index;
        Format = format;
        Alignment = alignment;
        ParsingType = parsingType;

        if(int.TryParse(name, NumberStyles.None, CultureInfo.InvariantCulture, out var specifyIndex))
        {
            ParameterIndex = specifyIndex;
        }
    }

    public string Name { get; }

    public string? Format { get; }

    /// <summary>
    /// 对其方式
    /// </summary>
    public Alignment? Alignment { get; }

    public PropertyTokenType ParsingType { get; }

    /// <summary>
    /// 属性索引
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// 参数索引
    /// </summary>
    public int ParameterIndex { get; }

    public override int GetHashCode() => Name.GetHashCode();

    internal void ResetPosition(int position)
    {
        Position = position;
    }
}
