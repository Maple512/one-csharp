namespace OneI;

using System;
using System.Diagnostics;

// source: https://github.com/dotnet/aspnetcore/blob/main/src/Shared/ValueStopwatch/ValueStopwatch.cs
public readonly struct StopwatchValue
{
    private static readonly long _timestampToTicks = TimeSpan.TicksPerSecond / Stopwatch.Frequency;

    private readonly long _startTimestamp;

    public bool IsActive => _startTimestamp != 0;

    private StopwatchValue(long startTimestamp) => _startTimestamp = startTimestamp;

    public static StopwatchValue StartNew() => new(Stopwatch.GetTimestamp());

    public TimeSpan GetElapsedTime()
    {
        // Start timestamp can't be zero in an initialized StopwatchValue. It would have to be
        // literally the first thing executed when the machine boots to be 0. So it being 0 is a
        // clear indication of default(StopwatchValue)
        if(!IsActive)
        {
            throw new InvalidOperationException($"An uninitialized, or 'default', {nameof(StopwatchValue)} cannot be used to get elapsed time.");
        }

        var end = Stopwatch.GetTimestamp();

        var timestampDelta = end - _startTimestamp;

        var ticks = _timestampToTicks * timestampDelta;

        return new TimeSpan(ticks);
    }
}
