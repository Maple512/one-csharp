namespace OneI.Openable.Internal;

using System.Security.Authentication;
using Microsoft.Extensions.Configuration;
using OneI.Openable.Https;
using static OpenableConstants.Configurations;

internal sealed class ConfigurationReader
{
    private readonly IConfiguration _configuration;

    private IDictionary<string, CertificateOptions>? _certificates;
    private EndpointDefaultsOptions? _endpointDefaults;
    private IEnumerable<EndPointOptions>? _endpoints;

    public ConfigurationReader(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public IDictionary<string, CertificateOptions> Certificates => _certificates ??= BuildCertificates();
    public EndpointDefaultsOptions EndpointDefaults => _endpointDefaults ??= BuildEndpointDefaults();
    public IEnumerable<EndPointOptions> Endpoints => _endpoints ??= BuildEndpoints();

    // "EndpointDefaults": {
    //     "Protocols": "Http1AndHttp2",
    //     "SslProtocols": [ "Tls11", "Tls12", "Tls13"],
    //     "ClientCertificateMode" : "NoCertificate"
    // }
    private EndpointDefaultsOptions BuildEndpointDefaults()
    {
        var configSection = _configuration.GetSection(EndpointDefaultsKey);

        return new EndpointDefaultsOptions
        {
            Protocols = ParseProtocols(configSection[ProtocolsKey]),
            SslProtocols = ParseSslProcotols(configSection.GetSection(SslProtocolsKey)),
            ClientCertificateMode = ParseClientCertificateMode(configSection[ClientCertificateModeKey]),
        };
    }

    // "EndpointName": {
    //     "Url": "https://*:5463",
    //     "Protocols": "Http1AndHttp2",
    //     "SslProtocols": [ "Tls11", "Tls12", "Tls13"],
    //     "Certificate": {
    //         "Path": "testCert.pfx",
    //         "Password": "testPassword"
    //     },
    //     "ClientCertificateMode" : "NoCertificate",
    //     "Sni": {
    //         "a.example.org": {
    //             "Certificate": {
    //                 "Path": "testCertA.pfx",
    //                 "Password": "testPassword"
    //             }
    //         },
    //         "*.example.org": {
    //             "Protocols": "Http1",
    //         }
    //     }
    // }
    private IEnumerable<EndPointOptions> BuildEndpoints()
    {
        var endpoints = new List<EndPointOptions>();

        var endpointsConfig = _configuration.GetSection(EndpointsKey).GetChildren();

        foreach(var endpointConfig in endpointsConfig)
        {
            var url = endpointConfig[UrlKey];
            if(string.IsNullOrEmpty(url))
            {
                throw new InvalidOperationException($"The endpoint {endpointConfig.Key} is missing the required 'Url' parameter.");
            }

            var endpoint = new EndPointOptions(
                endpointConfig.Key,
                url,
                ReadSni(endpointConfig.GetSection(SniKey), endpointConfig.Key),
                endpointConfig)
            {
                Protocols = ParseProtocols(endpointConfig[ProtocolsKey]),
                SslProtocols = ParseSslProcotols(endpointConfig.GetSection(SslProtocolsKey)),
                ClientCertificateMode = ParseClientCertificateMode(endpointConfig[ClientCertificateModeKey]),
                Certificate = new CertificateOptions(endpointConfig.GetSection(CertificateKey))
            };

            endpoints.Add(endpoint);
        }

        return endpoints;
    }

    // "Sni": {
    //     "a.example.org": {
    //         "Protocols": "Http1",
    //         "SslProtocols": [ "Tls11", "Tls12", "Tls13"],
    //         "Certificate": {
    //             "Path": "testCertA.pfx",
    //             "Password": "testPassword"
    //         },
    //         "ClientCertificateMode" : "NoCertificate"
    //     },
    //     "*.example.org": {
    //         "Certificate": {
    //             "Path": "testCertWildcard.pfx",
    //             "Password": "testPassword"
    //         }
    //     }
    //     // The following should work once https://github.com/dotnet/runtime/issues/40218 is resolved
    //     "*": {}
    // }
    private static Dictionary<string, SniOptions> ReadSni(IConfigurationSection sniConfig, string endpointName)
    {
        var sniDictionary = new Dictionary<string, SniOptions>(0, StringComparer.OrdinalIgnoreCase);

        foreach(var sniChild in sniConfig.GetChildren())
        {
            if(string.IsNullOrEmpty(sniChild.Key))
            {
                throw new InvalidOperationException($"The endpoint {endpointName} is invalid because an SNI configuration section has an empty string as its key. Use a wildcard ('*') SNI section to match all server names.");
            }

            var sni = new SniOptions
            {
                Certificate = new CertificateOptions(sniChild.GetSection(CertificateKey)),
                HttpProtocols = ParseProtocols(sniChild[ProtocolsKey]),
                SslProtocols = ParseSslProcotols(sniChild.GetSection(SslProtocolsKey)),
                ClientCertificateMode = ParseClientCertificateMode(sniChild[ClientCertificateModeKey])
            };

            sniDictionary.Add(sniChild.Key, sni);
        }

        return sniDictionary;
    }

    private IDictionary<string, CertificateOptions> BuildCertificates()
    {
        var certificates = new Dictionary<string, CertificateOptions>(0, StringComparer.OrdinalIgnoreCase);

        var children = _configuration.GetSection(CertificateKey).GetChildren();

        foreach(var child in children)
        {
            certificates.Add(child.Key, new(child));
        }

        return certificates;
    }

    private static ClientCertificateMode? ParseClientCertificateMode(string? clientCertificateMode)
    {
        if(Enum.TryParse<ClientCertificateMode>(clientCertificateMode, ignoreCase: true, out var result))
        {
            return result;
        }

        return null;
    }

    private static HttpProtocols? ParseProtocols(string? value)
    {
        if(Enum.TryParse<HttpProtocols>(value, true, out var result))
        {
            return result;
        }

        return null;
    }

    private static SslProtocols? ParseSslProcotols(IConfigurationSection sslProtocols)
    {
        // Avoid trimming warning from IConfigurationSection.Get<string[]>()
        string[]? stringProtocols = null;

        var childrenSections = sslProtocols.GetChildren().ToArray();

        if(childrenSections.Length > 0)
        {
            stringProtocols = new string[childrenSections.Length];

            for(var i = 0; i < childrenSections.Length; i++)
            {
                stringProtocols[i] = childrenSections[i].Value!;
            }
        }

        return stringProtocols?.Aggregate(SslProtocols.None, (acc, current) =>
        {
            if(Enum.TryParse(current, true, out SslProtocols parsed))
            {
                return acc | parsed;
            }

            return acc;
        });
    }

    internal static void ThrowIfContainsHttpsOnlyConfiguration(EndPointOptions endpoint)
    {
        if(endpoint.Certificate != null && (endpoint.Certificate.IsFile || endpoint.Certificate.IsStore))
        {
            throw new InvalidOperationException($"The non-HTTPS endpoint {endpoint.Name} includes HTTPS-only configuration for {CertificateKey}.");
        }

        if(endpoint.ClientCertificateMode.HasValue)
        {
            throw new InvalidOperationException($"The non-HTTPS endpoint {endpoint.Name} includes HTTPS-only configuration for {ClientCertificateModeKey}.");
        }

        if(endpoint.SslProtocols.HasValue)
        {
            throw new InvalidOperationException($"The non-HTTPS endpoint {endpoint.Name} includes HTTPS-only configuration for {SslProtocolsKey}.");
        }

        if(endpoint.Sni.Count > 0)
        {
            throw new InvalidOperationException($"The non-HTTPS endpoint {endpoint.Name} includes HTTPS-only configuration for {SniKey}.");
        }
    }
}
