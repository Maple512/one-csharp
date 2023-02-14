namespace OneI.Diagnostics;

/// <summary>
/// 统一的时钟
/// </summary>
public static class Clock
{
    private static Func<DateTimeOffset>? _provider;

    public static void Initialize(Func<DateTimeOffset> provider)
    {
        Check.ThrowIfNull(provider);

        _provider = provider;
    }

    public static DateTimeOffset Now
    {
        get
        {
            return _provider?.Invoke() ?? DateTimeOffset.Now;
        }
    }
}
