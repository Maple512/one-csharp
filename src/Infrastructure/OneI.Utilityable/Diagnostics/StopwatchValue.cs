namespace OneI.Diagnostics;

/// <summary>
/// 秒表
/// </summary>
/// <remarks>source: <see href="https://github.com/dotnet/aspnetcore/blob/main/src/Shared/ValueStopwatch/ValueStopwatch.cs"/></remarks>
#pragma warning disable CS0659 // “StopwatchValue”重写 Object.Equals(object o) 但不重写 Object.GetHashCode()
public readonly struct StopwatchValue : IEquatable<StopwatchValue>
#pragma warning restore CS0659 // “StopwatchValue”重写 Object.Equals(object o) 但不重写 Object.GetHashCode()
{
    private static readonly long _timestampToTicks = TimeSpan.TicksPerSecond / Stopwatch.Frequency;

    private readonly long _startTimestamp;

    public bool IsActive
    {
        get
        {
            return _startTimestamp != 0;
        }
    }

    private StopwatchValue(long startTimestamp) => _startTimestamp = startTimestamp;

    public static StopwatchValue StartNew()
    {
        return new(Stopwatch.GetTimestamp());
    }

    public StopwatchValue()
    {
        _startTimestamp = Stopwatch.GetTimestamp();
    }

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

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is StopwatchValue sv && Equals(sv);
    }

    public bool Equals(StopwatchValue other)
    {
        return IsActive && other.IsActive
            && _startTimestamp == other._startTimestamp;
    }
}
