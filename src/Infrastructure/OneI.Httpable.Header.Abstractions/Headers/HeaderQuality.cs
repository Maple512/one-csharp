namespace OneI.Httpable.Headers;

/// <summary>
/// 提供HTTP标头权重
/// </summary>
public sealed class HeaderQuality
{
    /// <summary>
    /// Quality factor to indicate a perfect match.
    /// </summary>
    public const double Match = 1.0;

    /// <summary>
    /// Quality factor to indicate no match.
    /// </summary>
    public const double NoMatch = 0.0;
}
