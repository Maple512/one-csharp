namespace OneI.Logable.Internal;

using OneI.Logable;

internal static class RollFrequencyExtensions
{
    public static string GetFormat(this RollFrequency interval)
        => interval switch
        {
            RollFrequency.Year => "yyyy",
            RollFrequency.Month => "yyyyMM",
            RollFrequency.Day => "yyyyMMdd"
               ,
            RollFrequency.Hour => "yyyyMMddHH",
            RollFrequency.Minute => "yyyyMMddHHmm"
               ,
            _ => throw new ArgumentException("Invalid rolling interval"),
        };

    /// <summary>
    ///     根据滚动频率获取周期时段
    /// </summary>
    /// <param name="interval"></param>
    /// <param name="datetime"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static DateTime GetCurrentPeriod(this RollFrequency interval, DateTime datetime)
        => interval switch
        {
            RollFrequency.Year => new DateTime(datetime.Year, 1, 1, 0, 0, 0, datetime.Kind),
            RollFrequency.Month => new DateTime(datetime.Year, datetime.Month, 1, 0, 0, 0, datetime.Kind),
            RollFrequency.Day => new DateTime(datetime.Year, datetime.Month, datetime.Day, 0, 0, 0, datetime.Kind),
            RollFrequency.Hour => new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, 0, 0
                                                    , datetime.Kind),
            RollFrequency.Minute => new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour
                                                      , datetime.Minute, 0, datetime.Kind),
            _ => throw new ArgumentException("Invalid rolling interval"),
        };

    /// <summary>
    ///     根据滚动频率获取下一个周期时段
    /// </summary>
    /// <param name="interval"></param>
    /// <param name="datetime"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static DateTime GetNextPeriod(this RollFrequency interval, DateTime datetime)
    {
        var current = interval.GetCurrentPeriod(datetime);

        return interval switch
        {
            RollFrequency.Year => current.AddYears(1),
            RollFrequency.Month => current.AddMonths(1),
            RollFrequency.Day => current.AddDays(1),
            RollFrequency.Hour => current.AddHours(1),
            RollFrequency.Minute => current.AddMinutes(1),
            _ => throw new ArgumentException("Invalid roll frequency"),
        };
    }
}
