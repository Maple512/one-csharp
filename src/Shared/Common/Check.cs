namespace System;

using Runtime.ExceptionServices;

[StackTraceHidden]
internal static class Check
{
    public static void ReThrow(this Exception exception)
    {
        ExceptionDispatchInfo.Capture(exception).Throw();
    }

    public static void ThrowIfNullOrWhiteSpace(string? value)
    {
        if(string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(nameof(value));
        }
    }

    public static void ThrowIfNull<T>(T? value)
    {
        if(value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }
    }

    public static void ThrowIfNullOrEmpty<T>(IEnumerable<T>? source)
    {
        if(source is null || source.Any() == false)
        {
            throw new ArgumentNullException(nameof(source));
        }
    }
}
