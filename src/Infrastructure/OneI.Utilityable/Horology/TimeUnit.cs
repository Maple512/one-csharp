namespace OneI.Horology;

// source from: https://github.com/AndreyAkinshin/perfolizer
public readonly struct TimeUnit : IEquatable<TimeUnit?>
{
    public string Name { get; }

    public string Description { get; }
    public long NanosecondAmount { get; }

    private TimeUnit(string name, string description, long nanosecondAmount)
    {
        Name = name;
        Description = description;
        NanosecondAmount = nanosecondAmount;
    }

    public TimeInterval ToInterval(long value = 1) => new(value, this);

    public static readonly TimeUnit Nanosecond = new("ns", "Nanosecond", 1);
    public static readonly TimeUnit Microsecond = new("\u03BCs", "Microsecond", 1000);
    public static readonly TimeUnit Millisecond = new("ms", "Millisecond", 1000 * 1000);
    public static readonly TimeUnit Second = new("s", "Second", 1000 * 1000 * 1000);
    public static readonly TimeUnit Minute = new("m", "Minute", Second.NanosecondAmount * 60);
    public static readonly TimeUnit Hour = new("h", "Hour", Minute.NanosecondAmount * 60);
    public static readonly TimeUnit Day = new("d", "Day", Hour.NanosecondAmount * 24);
    public static readonly TimeUnit[] All = { Nanosecond, Microsecond, Millisecond, Second, Minute, Hour, Day };

    /// <summary>
    /// This method chooses the best time unit for representing a set of time measurements.
    /// </summary>
    /// <param name="values">The list of time measurements in nanoseconds.</param>
    /// <returns>Best time unit.</returns>
    public static TimeUnit GetBestTimeUnit(params double[] values)
    {
        if(values.Length == 0)
        {
            return Nanosecond;
        }
        // Use the largest unit to display the smallest recorded measurement without loss of precision.
        var minValue = values.Min();
        foreach(var timeUnit in All)
        {
            if(minValue < timeUnit.NanosecondAmount * 1000)
            {
                return timeUnit;
            }
        }

        return All.Last();
    }

    public static double Convert(double value, TimeUnit from, TimeUnit? to)
        => value * from.NanosecondAmount
        / (to ?? GetBestTimeUnit(value)).NanosecondAmount;

    public bool Equals([NotNullWhen(true)] TimeUnit? other)
    {
        if(other is null)
        {
            return false;
        }

        return Name.Equals(other.Value.Name, StringComparison.InvariantCulture)
            && Description.Equals(other.Value.Description, StringComparison.InvariantCulture)
            && NanosecondAmount.Equals(other.Value.NanosecondAmount);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is TimeUnit tu && Equals(tu);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Description, NanosecondAmount);
    }

    public static bool operator ==(TimeUnit left, TimeUnit right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(TimeUnit left, TimeUnit right)
    {
        return !Equals(left, right);
    }
}
