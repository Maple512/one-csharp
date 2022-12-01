namespace OneI.Logable.Rolling;

/// <summary>
/// 文件滚动策略
/// </summary>
public enum RollingPolicy
{
    /// <summary>
    /// 不会滚动
    /// </summary>
    Infinite,
    Year, Month, Day, Hour, Minute
}
