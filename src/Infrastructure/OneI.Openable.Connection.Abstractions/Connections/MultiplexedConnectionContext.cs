namespace OneI.Openable.Connections;

public abstract class MultiplexedConnectionContext : ConnectionContextBase, IAsyncDisposable
{
    public abstract ValueTask<ConnectionContext?> AcceptAsync(CancellationToken cancellationToken = default);

    public abstract ValueTask<ConnectionContext?> ConnectAsync(CancellationToken cancellationToken = default);
}
