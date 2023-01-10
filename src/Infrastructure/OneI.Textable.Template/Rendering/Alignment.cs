namespace OneI.Textable.Rendering;

/// <summary>
/// 描述文本的对齐方向和宽度
/// </summary>
public readonly struct Alignment
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Alignment"/> class.
    /// </summary>
    /// <param name="width">The width.</param>
    public Alignment(int width)
    {
        Direction = width >= 0 ? Direction.Right : Direction.Left;
        Width = Math.Abs(width);
    }

    /// <summary>
    /// 描述文本对齐的方向，填充另一边
    /// </summary>
    public Direction Direction { get; }

    /// <summary>
    /// 整体的宽度
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Tos the string.
    /// </summary>
    /// <returns>A string.</returns>
    public override string ToString()
    {
        return $"{Direction}, {Width}";
    }
}
