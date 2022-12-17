namespace OneI.Diagnostics;

using System.Collections;
using System.Collections.Generic;

public static class DebugWatch
{
    private static readonly long _timestampToTicks = TimeSpan.TicksPerSecond / Stopwatch.Frequency;

    private static List<(MarkLocaltion, TimeSpan)> _marks;
    private static long _marked;
    private static long _start;

    private static Action<TimeSpan>? _receiver;

    public static void Start(Action<TimeSpan>? tick = null)
    {
        _marks = new();
        _start = _marked = Stopwatch.GetTimestamp();
        _receiver = tick;
    }

    [Conditional("DEBUG")]
    public static void Mark(
        string? category = null,
        [CallerFilePath] string? file = null,
        [CallerMemberName] string? member = null,
        [CallerLineNumber] int? line = null)
    {
        var mark = GetElapsedTime(_marked);
        _marks.Add((new MarkLocaltion(category, file, member, line), mark));
        _receiver?.Invoke(mark);

        _marked = Stopwatch.GetTimestamp();
    }

    [Conditional("DEBUG")]
    public static void EndAndReport(
        string? category = null,
        [CallerFilePath] string? file = null,
        [CallerMemberName] string? member = null,
        [CallerLineNumber] int? line = null)
    {
        var mark = GetElapsedTime(_marked);
        _marks.Add((new MarkLocaltion(category, file, member, line), mark));
        _receiver?.Invoke(mark);

        Debug.WriteLine($"总用时：{GetElapsedTime(_start).TotalMilliseconds:N2}ms");

        foreach(var point in _marks)
        {
            Debug.WriteLine($"    {point.Item2.TotalMilliseconds:N2}ms  -{point.Item1.ToSortString()}");
        }
    }

    readonly record struct MarkLocaltion(string? Category, string? FilePath, string? MemberName, int? LineNumber)
    {
        public string ToSortString()
        {
            return $"{(Category is null ? string.Empty : $"{Category} ")}{Path.GetFileNameWithoutExtension(FilePath)}#L{LineNumber}@{MemberName}";
        }
    }

    public static TimeSpan GetElapsedTime(long start)
    {
        var end = Stopwatch.GetTimestamp();

        var timestampDelta = end - start;

        var ticks = _timestampToTicks * timestampDelta;

        return new TimeSpan(ticks);
    }
}
