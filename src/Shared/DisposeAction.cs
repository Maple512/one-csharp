namespace OneI;

using System;

#if NET7_0_OR_GREATER
using System.Threading.Tasks;
#endif

/// <summary>
/// Dispose action
/// </summary>
internal readonly partial struct DisposeAction : IDisposable
{
    private readonly Action? _action;

    public static readonly DisposeAction Nullable = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposeAction"/> class.
    /// </summary>
    /// <param name="action">The action.</param>
    public DisposeAction(Action action) => _action = action;

    /// <summary>
    /// Disposes the.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _action?.Invoke();
    }
}

#if NET7_0_OR_GREATER
internal readonly partial struct DisposeAction : IAsyncDisposable
{
    /// <summary>
    /// Disposes the async.
    /// </summary>
    /// <returns>A ValueTask.</returns>
    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        _action?.Invoke();

        return ValueTask.CompletedTask;
    }
}
#endif
