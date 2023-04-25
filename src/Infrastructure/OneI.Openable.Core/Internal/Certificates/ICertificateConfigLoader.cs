namespace OneI.Openable.Internal.Certificates;

using System.Security.Cryptography.X509Certificates;

internal interface ICertificateConfigLoader
{
    bool IsTestMock { get; }

    (X509Certificate2?, X509Certificate2Collection?) LoadCertificate(CertificateOptions options, string endpointName);
}
