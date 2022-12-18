namespace OneI;

using System;
using System.Threading.Tasks;

internal readonly struct DisposeAction : IDisposable, IAsyncDisposable
{
    private readonly Action? _action;

    public static readonly DisposeAction Nullable = new();

    public DisposeAction(Action action) => _action = action;

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _action?.Invoke();
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        _action?.Invoke();

        return ValueTask.CompletedTask;
    }
}
