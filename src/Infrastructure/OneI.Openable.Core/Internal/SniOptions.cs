namespace OneI.Openable.Internal;

using System.Security.Authentication;
using OneI.Openable.Https;

internal sealed class SniOptions
{
    public HttpProtocols? HttpProtocols { get; set; }

    public SslProtocols? SslProtocols { get; set; }

    public CertificateOptions? Certificate { get; set; }

    public ClientCertificateMode? ClientCertificateMode { get; set; }

    public override bool Equals(object? obj)
        => obj is SniOptions other &&
         (HttpProtocols ?? ListenOptions.HttpProtocolDefault) == (other.HttpProtocols ?? ListenOptions.HttpProtocolDefault)
        && (SslProtocols ?? System.Security.Authentication.SslProtocols.None) == (other.SslProtocols ?? System.Security.Authentication.SslProtocols.None)
        && Certificate == other.Certificate
        && (ClientCertificateMode ?? Https.ClientCertificateMode.No) == (other.ClientCertificateMode ?? Https.ClientCertificateMode.No);

    public override int GetHashCode() => HashCode.Combine(
        HttpProtocols ?? ListenOptions.HttpProtocolDefault, SslProtocols ?? System.Security.Authentication.SslProtocols.None,
        Certificate, ClientCertificateMode ?? Https.ClientCertificateMode.No);

    public static bool operator ==(SniOptions? lhs, SniOptions? rhs)
    {
        return lhs is null ? rhs is null : lhs.Equals(rhs);
    }

    public static bool operator !=(SniOptions? lhs, SniOptions? rhs)
    {
        return !(lhs == rhs);
    }
}
