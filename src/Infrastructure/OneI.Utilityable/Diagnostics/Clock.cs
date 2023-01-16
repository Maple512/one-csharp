namespace OneI.Diagnostics;

/// <summary>
/// 统一的时钟
/// </summary>
public static class Clock
{
    private static Func<DateTimeOffset>? _provider;

    public static void Initialize(Func<DateTimeOffset> provider)
    {
        if(_provider != null)
        {
            throw new Exception($"The initialize method can only be called once.");
        }

        _provider = Check.NotNull(provider);
    }

    public static DateTimeOffset Now => _provider?.Invoke() ?? DateTimeOffset.Now;
}
