namespace OneI.Applicationable;

/// <summary>
/// 表示在某一个<see cref="BackgroundService"/>发生异常时应遵循的行为
/// </summary>
public enum BackgroundServiceExceptionBehavior : byte
{
    Stop,

    Ignore
}
