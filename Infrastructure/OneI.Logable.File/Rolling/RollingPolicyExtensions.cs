namespace OneI.Logable.Rolling;

using System;

public static class RollingPolicyExtensions
{
    public static string GetFormatString(this RollingPolicy policy)
        => policy switch
        {
            RollingPolicy.Motionless => string.Empty,
            RollingPolicy.Year => "yyyy",
            RollingPolicy.Month => "yyyyMM",
            RollingPolicy.Day => "yyyyMMdd",
            RollingPolicy.Hour => "yyyyMMddHH",
            RollingPolicy.Minute => "yyyyMMddHHmm",
            _ => throw new ArgumentException("Invalid rolling policy"),
        };

    public static DateTime? GetCurrentCheckPoint(this RollingPolicy policy, DateTime datetime)
        => policy switch
        {
            RollingPolicy.Motionless => null,
            RollingPolicy.Year => new DateTime(datetime.Year, 1, 1, 0, 0, 0, datetime.Kind),
            RollingPolicy.Month => new DateTime(datetime.Year, datetime.Month, 1, 0, 0, 0, datetime.Kind),
            RollingPolicy.Day => new DateTime(datetime.Year, datetime.Month, datetime.Day, 0, 0, 0, datetime.Kind),
            RollingPolicy.Hour => new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, 0, 0, datetime.Kind),
            RollingPolicy.Minute => new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, 0, datetime.Kind),
            _ => throw new ArgumentException("Invalid rolling policy")
        };

    public static DateTime? GetNextCheckpoint(this RollingPolicy interval, DateTime instant)
    {
        var current = GetCurrentCheckPoint(interval, instant);

        if(current == null)
        {
            return null;
        }

        return interval switch
        {
            RollingPolicy.Year => (DateTime?)current.Value.AddYears(1),
            RollingPolicy.Month => (DateTime?)current.Value.AddMonths(1),
            RollingPolicy.Day => (DateTime?)current.Value.AddDays(1),
            RollingPolicy.Hour => (DateTime?)current.Value.AddHours(1),
            RollingPolicy.Minute => (DateTime?)current.Value.AddMinutes(1),
            _ => throw new ArgumentException("Invalid rolling interval"),
        };
    }
}
