namespace OneI.Openable.Connections;

using System.Net.Security;

/// <summary>
/// TLS的连接状态
/// </summary>
public sealed class TlsConnectionCallbackContext
{
    public SslClientHelloInfo ClientHelloInfo { get; set; }

    public object? State { get; set; }

    public ConnectionContextBase ConnectionContext { get; set; } = default!;
}
