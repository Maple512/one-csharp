namespace System;

using System.Runtime.ExceptionServices;

[DebuggerStepThrough]
internal static partial class ExceptionExtensions
{

}

#if NET7_0_OR_GREATER
[StackTraceHidden]
internal static partial class ExceptionExtensions
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
#elif NETSTANDARD2_0_OR_GREATER
internal static partial class ExceptionExtensions
{
    /// <summary>
    /// 抛出原始异常
    /// </summary>
    /// <param name="exception"></param>
    public static void ReThrow(this Exception exception)
    {
        ExceptionDispatchInfo.Capture(exception).Throw();
    }
}
#endif
