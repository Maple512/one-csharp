namespace OneI.Logable.Parsing;

using System.Globalization;
using OneI.Logable.Rendering;

public class PropertyToken : TextToken
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
        Index = index;
        Format = format;
        Alignment = alignment;
        ParsingType = parsingType;

        if(int.TryParse(name, NumberStyles.None, CultureInfo.InvariantCulture, out var specifyIndex))
        {
            SpecifyIndex = specifyIndex;
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
    /// 是否直接指定索引，例如：<c>{1}</c>
    /// 如果直接指定索引，则按索引寻找参数
    /// </summary>
    public int Index { get; }

    public int? SpecifyIndex { get; }

    public override int GetHashCode() => Name.GetHashCode();

    internal void ResetPosition(int position)
    {
        Position = position;
    }
}
