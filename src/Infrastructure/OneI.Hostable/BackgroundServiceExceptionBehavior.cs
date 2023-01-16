namespace OneI.Hostable;

/// <summary>
/// 表示在某一个<see cref="BackgroundService"/>发生异常时应遵循的行为
/// </summary>
public enum BackgroundServiceExceptionBehavior
{
    /// <summary>
    /// 停止<see cref="IHost"/>
    /// </summary>
    StopHost,

    /// <summary>
    /// 忽略发生的异常
    /// </summary>
    Ignore
}
