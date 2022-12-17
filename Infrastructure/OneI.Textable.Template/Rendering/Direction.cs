namespace OneI.Textable.Rendering;

/// <summary>
/// 对齐方向
/// </summary>
public enum Direction
{
    None,
    /// <summary>
    /// 左对齐
    /// <code>
    /// message******
    /// messageee****
    /// </code>
    /// </summary>
    Left,

    /// <summary>
    /// 右对齐
    /// <code>
    /// ******message
    /// ****messageee
    /// </code>
    /// </summary>
    Right,
}
