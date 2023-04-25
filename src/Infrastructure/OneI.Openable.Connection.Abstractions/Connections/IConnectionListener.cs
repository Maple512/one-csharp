namespace OneI.Openable.Connections;

using System.Net;

/// <summary>
/// 特定端口的监听器
/// </summary>
public interface IConnectionListener : IAsyncDisposable
{
    EndPoint EndPoint { get; }

    ValueTask<ConnectionContext?> AcceptAsync(CancellationToken cancellationToken = default);

    ValueTask UnbindAsync(CancellationToken cancellationToken = default);
}
