namespace OneI.Utilityable;

using System;
#if DEBUG
using System.Threading;
#endif

public static class Clock
{
    private static Func<DateTime> _nowAction = () => DateTime.Now;
    private static Func<DateTime> _utcNowAction = () => DateTime.UtcNow;
    private static Func<DateTimeOffset> _offsetNowAction = () => DateTimeOffset.Now;

    public static DateTime Now => _nowAction();

    public static DateTime UtcNow => _utcNowAction();

    public static DateTimeOffset OffsetNow => _offsetNowAction();

#if DEBUG

    private static readonly AsyncLocal<DateTime> _datetimeLocal = new();

    public static void OverrideDataTime(DateTime dateTime)
    {
        _datetimeLocal.Value = dateTime;

        _nowAction = static () => _datetimeLocal.Value;
    }

    private static readonly AsyncLocal<DateTime> _datetimeUtcLocal = new();

    public static void OverrideDataTimeUtc(DateTime dateTime)
    {
        _datetimeUtcLocal.Value = dateTime;

        _utcNowAction = static () => _datetimeUtcLocal.Value;
    }

    private static readonly AsyncLocal<DateTimeOffset> _datetimeOffsetLocal = new();

    public static void OverrideDataTimeOffset(DateTimeOffset dateTime)
    {
        _datetimeOffsetLocal.Value = dateTime;

        _offsetNowAction = static () => _datetimeOffsetLocal.Value;
    }

#endif
}
