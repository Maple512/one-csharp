namespace OneI.Openable.Internal;

using OneI.Openable.Https;
using System.Security.Authentication;

internal sealed class EndpointDefaultsOptions
{
    public HttpProtocols? Protocols { get; set; }

    public SslProtocols? SslProtocols { get; set; }

    public ClientCertificateMode? ClientCertificateMode { get; set; }
}
