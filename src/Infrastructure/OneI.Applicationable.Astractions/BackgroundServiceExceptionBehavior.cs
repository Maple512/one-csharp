namespace OneI.Applicationable;

/// <summary>
/// 指示应用后台服务异常后的行为
/// </summary>
public enum BackgroundServiceExceptionBehavior
{
    /// <summary>
    /// 整个应用停止
    /// </summary>
    Stop,

    /// <summary>
    /// 忽略发生的异常
    /// </summary>
    Ignore,
}
