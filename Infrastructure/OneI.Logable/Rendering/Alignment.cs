namespace OneI.Logable.Rendering;

public readonly struct Alignment
{
    public Alignment(int width)
    {
        Direction = width >= 0 ? Direction.Right : Direction.Left;
        Width = width;
    }

    /// <summary>
    /// 描述文本对齐的方向，填充另一边
    /// </summary>
    public Direction Direction { get; }

    /// <summary>
    /// 整体的宽度
    /// </summary>
    public int Width { get; }

    public override string ToString()
    {
        return $"{Direction}, {Width}";
    }
}
