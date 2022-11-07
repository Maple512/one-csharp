namespace System.Diagnostics;

using System;

public readonly struct ProcessResult
{
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
