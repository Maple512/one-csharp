namespace OneI.Diagnostics;

/// <summary>
/// 秒表
/// </summary>
/// <remarks>source: https://github.com/dotnet/aspnetcore/blob/main/src/Shared/ValueStopwatch/ValueStopwatch.cs</remarks>
public readonly struct StopwatchValue
{
    private static readonly long _timestampToTicks = TimeSpan.TicksPerSecond / Stopwatch.Frequency;

    private readonly long _startTimestamp;

    public bool IsActive => _startTimestamp != 0;

    private StopwatchValue(long startTimestamp) => _startTimestamp = startTimestamp;

    /// <summary>
    /// 开始一个新的计时
    /// </summary>
    /// <returns></returns>
    public static StopwatchValue StartNew()
    {
        return new(Stopwatch.GetTimestamp());
    }

    /// <summary>
    /// 获取从开始调用<see cref="StartNew"/>到现在经过的时间
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public TimeSpan GetElapsedTime()
    {
        if(!IsActive)
        {
            throw new InvalidOperationException($"An uninitialized, Please call {nameof(StartNew)} method to initialize.");
        }

        var end = Stopwatch.GetTimestamp();

        var timestampDelta = end - _startTimestamp;

        var ticks = _timestampToTicks * timestampDelta;

        return new TimeSpan(ticks);
    }
}
