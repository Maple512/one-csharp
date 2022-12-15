namespace OneI;

/// <summary>
/// 时钟配置
/// </summary>
public static class Clock
{
    private static Func<DateTimeOffset>? _provider;

    public static void Initialize(Func<DateTimeOffset> provider)
    {
        if(_provider != null)
        {
            throw new InvalidOperationException($"The {nameof(Clock)}.{nameof(Initialize)} method can only be called once.");
        }

        _provider = Check.NotNull(provider);
    }

    public static DateTimeOffset DateTime => _provider?.Invoke() ?? DateTimeOffset.Now;
}
