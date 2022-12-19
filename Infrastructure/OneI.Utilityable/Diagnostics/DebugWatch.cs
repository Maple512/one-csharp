namespace OneI.Diagnostics;

using System.Collections.Generic;

public static class DebugWatch
{
    private static readonly long _timestampToTicks = TimeSpan.TicksPerSecond / Stopwatch.Frequency;

    private static List<DebugLocationRange> _marks;
    private static DebugLocaltion _begin;
    private static DebugLocaltion _end;
    private static string? _catagory;
    private static long _marked;
    private static long _start;
    private static bool _isStop;
    private static int _count;
    private static Action<int, TimeSpan>? _receiver;

    [Conditional("DEBUG")]
    public static void Start(
        Action<int, TimeSpan>? tick = null,
        string? category = null,
        [CallerFilePath] string? file = null,
        [CallerLineNumber] int? line = null)
    {
        _marks = new();
        _receiver = tick;
        _catagory = category;
        _begin = new DebugLocaltion(Path.GetFileNameWithoutExtension(file), line);
        _start = _marked = Stopwatch.GetTimestamp();
    }

    /// <summary>
    /// 暂时停止，不计入耗时
    /// </summary>
    [Conditional("DEBUG")]
    public static void Stop(
        [CallerFilePath] string? file = null,
        [CallerLineNumber] int? line = null)
    {
        if(_isStop)
        {
            return;
        }

        var elapsed = GetElapsedTime(_marked);
        _end = new DebugLocaltion(Path.GetFileNameWithoutExtension(file), line);
        _isStop = true;
        _count++;
        _marks.Add(new DebugLocationRange(_begin, _end, _catagory, elapsed));
        _begin = _end;
        _receiver?.Invoke(_count, elapsed);
        _marked = Stopwatch.GetTimestamp();
    }

    [Conditional("DEBUG")]
    public static void Mark(
        string? category = null,
        [CallerFilePath] string? file = null,
        [CallerLineNumber] int? line = null)
    {
        if(_isStop)
        {
            _marked = Stopwatch.GetTimestamp();

            _isStop = false;
            _catagory = category;
            _begin = new DebugLocaltion(Path.GetFileNameWithoutExtension(file), line);
        }
        else
        {
            var elapsed = GetElapsedTime(_marked);
            _end = new DebugLocaltion(Path.GetFileNameWithoutExtension(file), line);
            _count++;
            _marks.Add(new DebugLocationRange(_begin, _end, _catagory, elapsed));
            _catagory = category;
            _begin = _end;
            _receiver?.Invoke(_count, elapsed);
            _marked = Stopwatch.GetTimestamp();
        }
    }

    [Conditional("DEBUG")]
    public static void EndAndReport(
        string? category = null,
        [CallerFilePath] string? file = null,
        [CallerLineNumber] int? line = null)
    {
        Mark(category, file, line);

        for(var i = 0; i < _marks.Count; i++)
        {
            var mark = _marks[i];

            Debug.WriteLine($"{i + 1}: {mark}");
        }

        Debug.WriteLine($"总用时：{GetElapsedTime(_start).TotalMilliseconds:N2}ms");
    }

    public static TimeSpan GetElapsedTime(long start)
    {
        var timestampDelta = Stopwatch.GetTimestamp() - start;

        var ticks = _timestampToTicks * timestampDelta;

        return new TimeSpan(ticks);
    }
}

/// <summary>
/// Debug位置范围记录
/// </summary>
/// <param name="Begin">开始的位置</param>
/// <param name="End">结束的位置</param>
/// <param name="Elapsed">从开始到结束的总耗时</param>
internal record struct DebugLocationRange(DebugLocaltion Begin, DebugLocaltion End, string? Category, TimeSpan Elapsed)
{
    public override string ToString()
    {
        var builder = new StringBuilder();

        builder.Append($"{Elapsed.TotalMilliseconds:N4} (");

        if(Category != null)
        {
            builder.Append($"{Category}");
        }
        else
        {
            builder.Append($"{Begin} - {End}");
        }

        builder.Append(')');

        return builder.ToString();
    }
}

internal readonly record struct DebugLocaltion(string? FileName, int? LineNumber)
{
    public override string ToString()
    {
        return $"{FileName}#L{LineNumber}";
    }
}
