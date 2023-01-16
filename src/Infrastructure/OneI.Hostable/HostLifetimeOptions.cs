namespace OneI.Hostable;

public class HostLifetimeOptions
{
    /// <summary>
    /// 是否应抑制<see cref="IHost"/>生命期的状态消息
    /// </summary>
    public bool IsSuppressStatusMessages { get; set; } = false;
}
