namespace System;

using System.Runtime.ExceptionServices;

[StackTraceHidden]
[DebuggerStepThrough]
public static class ExceptionExtensions
{
    /// <summary>
    /// 抛出原始异常
    /// </summary>
    /// <param name="exception"></param>
    [DoesNotReturn]
    public static void ReThrow(this Exception exception)
    {
        ExceptionDispatchInfo.Capture(exception).Throw();
    }
}
