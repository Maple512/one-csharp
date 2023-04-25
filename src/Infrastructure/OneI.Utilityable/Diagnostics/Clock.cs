namespace OneI.Diagnostics;

public static class Clock
{
    private static Func<DateTimeOffset>? _provider;

    public static void Initialize(Func<DateTimeOffset> provider)
    {
        ThrowHelper.ThrowIfNull(provider);

        _provider = provider;
    }

    public static DateTimeOffset Now() => _provider?.Invoke() ?? DateTimeOffset.Now;
}
