namespace OneI.Diagnostics;

using System.ComponentModel;

/// <summary>
/// 秒表
/// </summary>
/// <remarks>source: <see href="https://github.com/dotnet/aspnetcore/blob/main/src/Shared/ValueStopwatch/ValueStopwatch.cs"/></remarks>
public readonly struct StopwatchValue : IEquatable<StopwatchValue>
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

    public static bool operator ==(StopwatchValue left, StopwatchValue right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(StopwatchValue left, StopwatchValue right)
    {
        return !(left == right);
    }

    [DoesNotReturn]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
