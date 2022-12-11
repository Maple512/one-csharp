namespace OneI;

public readonly struct DisposeAction : IDisposable
#if NET7_0_OR_GREATER
    , IAsyncDisposable
#endif
{
    private readonly Action? _action;

    public static readonly DisposeAction Nullable = new();

    public DisposeAction(Action action) => _action = action;

    public void Dispose()
    {
        _action?.Invoke();

        GC.SuppressFinalize(this);
    }
    #if NET7_0_OR_GREATER
    public ValueTask DisposeAsync()
    {
        _action?.Invoke();

        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
    }
    #endif
}
