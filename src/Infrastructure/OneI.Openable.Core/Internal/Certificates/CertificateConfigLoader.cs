namespace OneI.Openable.Internal.Certificates;

using System.Security.Cryptography.X509Certificates;

internal sealed class CertificateConfigLoader : ICertificateConfigLoader
{
    public bool IsTestMock { get; }

    public (X509Certificate2?, X509Certificate2Collection?) LoadCertificate(CertificateOptions options, string endpointName)
    {
        throw new NotImplementedException();
    }
}
