namespace OneI.Logable.Templatizations;

/// <summary>
/// 对齐方向
/// </summary>
public enum TextDirection
{
    None,
    /// <summary>
    /// 左对齐
    /// <code>
    /// message******
    /// text*********
    /// </code>
    /// </summary>
    Left,

    /// <summary>
    /// 右对齐
    /// <code>
    /// ******message
    /// *********text
    /// </code>
    /// </summary>
    Right,
}
