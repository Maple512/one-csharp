namespace System.Diagnostics;

public readonly struct ProcessResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessResult"/> class.
    /// </summary>
    /// <param name="processId">The process id.</param>
    /// <param name="exitedCode">The exited code.</param>
    /// <param name="totalProcessorTime">The total processor time.</param>
    public ProcessResult(
        int processId,
        int exitedCode,
        TimeSpan totalProcessorTime)
    {
        ProcessId = processId;
        ExitedCode = exitedCode;
        TotalProcessorTime = totalProcessorTime;
    }

    /// <summary>
    /// 进程ID
    /// </summary>
    public int ProcessId { get; }

    /// <summary>
    /// 退出码
    /// </summary>
    public int ExitedCode { get; }

    /// <summary>
    /// 处理器总时间
    /// </summary>
    public TimeSpan TotalProcessorTime { get; }
}
