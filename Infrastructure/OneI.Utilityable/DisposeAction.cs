namespace OneI;

using System;
using System.Threading.Tasks;

public readonly struct DisposeAction : IDisposable, IAsyncDisposable
{
    private readonly Action? _action;

    public static readonly DisposeAction Nullable = new();

    public DisposeAction(Action action)
    {
        _action = action;
    }

    public void Dispose()
    {
        _action?.Invoke();

        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        _action?.Invoke();

        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
    }
}

