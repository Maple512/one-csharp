namespace OneI.Openable.Connections;

using System.Net.Security;

public class TlsConnectionCallbackOptions
{
    public Func<TlsConnectionCallbackContext, CancellationToken, ValueTask<SslServerAuthenticationOptions>> OnConnection { get; set; } = default!;

    public object? OnConnectionState { get; set; }

    public List<SslApplicationProtocol> Protocols { get; set; } = default!;
}
