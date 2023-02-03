namespace OneI.Logable;

[OneI.Generateable.ToFastString]
public enum LogLevel : sbyte
{
    /// <summary>
    ///     包含最详细消息的日志。 这些消息可能包含敏感应用程序数据。 这些消息默认情况下处于禁用状态，并且绝不应在生产环境中启用
    /// </summary>
    Verbose,

    /// <summary>
    ///     在开发过程中用于交互式调查的日志。 这些日志应主要包含对调试有用的信息，并且没有长期价值
    /// </summary>
    Debug,

    /// <summary>
    ///     跟踪应用程序的常规流的日志。 这些日志应具有长期价值
    /// </summary>
    Information,

    /// <summary>
    ///     突出显示应用程序流中的异常或意外事件（不会导致应用程序执行停止）的日志
    /// </summary>
    Warning,

    /// <summary>
    ///     当前执行流因故障而停止时突出显示的日志。 这些日志指示当前活动中的故障，而不是应用程序范围内的故障
    /// </summary>
    Error,

    /// <summary>
    ///     描述不可恢复的应用程序/系统崩溃或需要立即引起注意的灾难性故障的日志
    /// </summary>
    Fatal,
}
