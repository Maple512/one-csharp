namespace OneI;

using System;

#if NET7_0_OR_GREATER
using System.Threading.Tasks;
#endif

internal readonly partial struct DisposeAction : IDisposable
{
    private readonly Action? _action;

    public static readonly DisposeAction Nullable = new();

    public DisposeAction(Action action) => _action = action;

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _action?.Invoke();
    }
}

#if NET7_0_OR_GREATER
internal readonly partial struct DisposeAction : IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        _action?.Invoke();

        return ValueTask.CompletedTask;
    }
}
#endif
