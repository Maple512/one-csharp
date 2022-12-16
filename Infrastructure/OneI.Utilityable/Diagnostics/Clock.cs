namespace OneI.Diagnostics;

/// <summary>
/// 统一的时钟
/// </summary>
public static class Clock
{
    private static Func<DateTimeOffset>? _provider;

    // first called at
    private static CalledLocation? _location;

    public static void Initialize(
        Func<DateTimeOffset> provider,
        [CallerMemberName] string? memeber = null,
        [CallerFilePath] string? file = null,
        [CallerLineNumber] int? line = null)
    {
        if(_provider != null)
        {
            throw new InitializationException(_location!.Value);
        }

        _location = new(memeber, file, line);

        _provider = Check.NotNull(provider);
    }

    public static DateTimeOffset Now => _provider?.Invoke() ?? DateTimeOffset.Now;
}
