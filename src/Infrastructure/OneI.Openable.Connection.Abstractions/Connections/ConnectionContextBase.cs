namespace OneI.Openable.Connections;

using System.Net;

public abstract class ConnectionContextBase : IAsyncDisposable
{
    public abstract string ConnectionId { get; set; }

    /// <summary>
    /// 客户端链接关闭时触发的令牌
    /// </summary>
    public virtual CancellationToken ClosedToken { get; set; }

    /// <summary>
    /// 此链接的本地端点
    /// </summary>
    public virtual EndPoint? LocalEndPoint { get; set; }

    /// <summary>
    /// 此链接的远程端点
    /// </summary>
    public virtual EndPoint? RemoteEndPoint { get; set; }

    /// <summary>
    /// 终止连接
    /// </summary>
    public abstract void Abort();

    /// <summary>
    /// 终止连接
    /// </summary>
    /// <param name="reason">终止的原因</param>
    public abstract void Abort(ConnectionAbortedException reason);

    public virtual ValueTask DisposeAsync()
    {
        return default;
    }
}
