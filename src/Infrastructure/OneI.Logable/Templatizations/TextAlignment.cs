namespace OneI.Logable.Rendering;

using OneI.Logable.Templatizations;

/// <summary>
/// 描述文本的对齐方向和宽度
/// </summary>
public readonly struct TextAlignment
{
    public TextAlignment(in int width)
    {
        Direction = width >= 0 ? TextDirection.Right : TextDirection.Left;
        Width = width;
    }

    public TextDirection Direction { get; }

    public int Width { get; }

    public override int GetHashCode() => HashCode.Combine(Direction, Width);

    public override string ToString() => $"[Direction: {Direction}, Width: {Width}]";
}
