namespace OneI;

#if NET
using System.Threading.Tasks;
#endif

internal readonly partial struct DisposeAction : IDisposable
{
    private readonly Action? _action;

    public static readonly DisposeAction Nullable = new();

    public DisposeAction(Action action) => _action = action;

    public void Dispose()
    {


        _action?.Invoke();
    }
}

#if NET
internal readonly partial struct DisposeAction : IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {


        _action?.Invoke();

        return ValueTask.CompletedTask;
    }
}
#endif

internal readonly partial struct DisposeAction<T> : IDisposable
{
    public static readonly DisposeAction<T> Nullable = new();

    private readonly Action<T>? _action;
    private readonly T _state;

    public DisposeAction(Action<T> action, T state)
    {
        _action = action;
        _state = state;
    }

    public void Dispose()
    {


        _action?.Invoke(_state);
    }
}

#if NET
internal readonly partial struct DisposeAction<T> : IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {


        _action?.Invoke(_state);

        return ValueTask.CompletedTask;
    }
}
#endif
