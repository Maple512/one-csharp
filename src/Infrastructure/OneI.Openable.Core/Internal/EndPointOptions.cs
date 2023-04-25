namespace OneI.Openable.Internal;

using System.Security.Authentication;
using Microsoft.Extensions.Configuration;
using OneI.Openable.Https;

internal sealed class EndPointOptions
{
    private readonly ConfigSectionClone _configSectionClone;

    public EndPointOptions(
        string name,
        string url,
        Dictionary<string, SniOptions> sni,
        IConfigurationSection configSection)
    {
        Name = name;
        Url = url;
        Sni = sni;

        // Compare config sections because it's accessible to app developers via an Action<EndpointConfiguration> callback.
        // We cannot rely entirely on comparing config sections for equality, because KestrelConfigurationLoader.Reload() sets
        // EndpointConfig properties to their default values. If a default value changes, the properties would no longer be equal,
        // but the config sections could still be equal.
        ConfigSection = configSection;

        // The IConfigrationSection will mutate, so we need to take a snapshot to compare against later and check for changes.
        _configSectionClone = new ConfigSectionClone(configSection);
    }

    public string Name { get; }
    public string Url { get; }
    public Dictionary<string, SniOptions> Sni { get; }
    public IConfigurationSection ConfigSection { get; }

    public HttpProtocols? Protocols { get; set; }
    public SslProtocols? SslProtocols { get; set; }
    public CertificateOptions? Certificate { get; set; }
    public ClientCertificateMode? ClientCertificateMode { get; set; }

    public override bool Equals(object? obj) =>
        obj is EndPointOptions other
        && Name == other.Name
        && Url == other.Url
        && (Protocols ?? ListenOptions.HttpProtocolDefault) == (other.Protocols ?? ListenOptions.HttpProtocolDefault)
        && (SslProtocols ?? System.Security.Authentication.SslProtocols.None) == (other.SslProtocols ?? System.Security.Authentication.SslProtocols.None)
        && Certificate == other.Certificate
        && (ClientCertificateMode ?? Https.ClientCertificateMode.No) == (other.ClientCertificateMode ?? Https.ClientCertificateMode.No)
        && CompareSniDictionaries(Sni, other.Sni)
        && _configSectionClone == other._configSectionClone;

    public override int GetHashCode() 
        => HashCode.Combine(
            Name,
            Url,
            Protocols ?? ListenOptions.HttpProtocolDefault,
            SslProtocols ?? System.Security.Authentication.SslProtocols.None,
            Certificate,
            ClientCertificateMode ?? Https.ClientCertificateMode.No,
            Sni.Count,
            _configSectionClone);

    public static bool operator ==(EndPointOptions? lhs, EndPointOptions? rhs)
    {
        return lhs is null ? rhs is null : lhs.Equals(rhs);
    }

    public static bool operator !=(EndPointOptions? lhs, EndPointOptions? rhs)
    {
        return !(lhs == rhs);
    }

    private static bool CompareSniDictionaries(Dictionary<string, SniOptions> lhs, Dictionary<string, SniOptions> rhs)
    {
        if(lhs.Count != rhs.Count)
        {
            return false;
        }

        foreach(var (lhsName, lhsSniConfig) in lhs)
        {
            if(!rhs.TryGetValue(lhsName, out var rhsSniConfig) || lhsSniConfig != rhsSniConfig)
            {
                return false;
            }
        }

        return true;
    }
}
