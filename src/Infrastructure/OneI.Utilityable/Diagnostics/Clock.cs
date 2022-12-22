namespace OneI.Diagnostics;

/// <summary>
/// 统一的时钟
/// </summary>
public static class Clock
{
    private static Func<DateTimeOffset>? _provider;

    // first called at
    private static CalledLocation? _location;

    /// <summary>
    /// Initializes the.
    /// </summary>
    /// <param name="provider">The provider.</param>
    /// <param name="file">The file.</param>
    /// <param name="memeber">The memeber.</param>
    /// <param name="line">The line.</param>
    public static void Initialize(
        Func<DateTimeOffset> provider,
        [CallerFilePath] string? file = null,
        [CallerMemberName] string? memeber = null,
        [CallerLineNumber] int? line = null)
    {
        if(_provider != null)
        {
            throw new Exception($"The initialize method can only be called once. First called at: {_location}.");
        }

        _location = new(file, memeber, line);

        _provider = Check.NotNull(provider);
    }

    /// <summary>
    /// Gets the now.
    /// </summary>
    public static DateTimeOffset Now => _provider?.Invoke() ?? DateTimeOffset.Now;
}
