namespace OneI.Openable.Internal;

using Microsoft.Extensions.Configuration;

internal sealed class CertificateOptions
{
    public CertificateOptions(IConfigurationSection section)
    {
        Section = section;

        Path = Section[nameof(Path)];
        KeyPath = Section[nameof(KeyPath)];
        Password = Section[nameof(Password)];
        Subject = Section[nameof(Subject)];
        Store = Section[nameof(Store)];
        Location = Section[nameof(Location)];

        if(bool.TryParse(Section[nameof(AllowInvalid)], out var value))
        {
            AllowInvalid = value;
        }
    }

    public IConfigurationSection? Section { get; }

    public string? Path { get; set; }

    public bool IsFile => Path.NotNullOrEmpty();

    public string? KeyPath { get; set; }

    public string? Password { get; set; }

    public string? Subject { get; set; }

    public string? Store { get; set; }

    public string? Location { get; set; }

    public bool? AllowInvalid { get; set; }

    public bool IsStore => Subject.NotNullOrEmpty();

    public override int GetHashCode()
    {
        return HashCode.Combine(Path, KeyPath, Password, Subject, Store, Location, AllowInvalid ?? false);
    }

    public override bool Equals(object? obj)
    {
        return obj is CertificateOptions other
            && Path == other.Path
            && KeyPath == other.KeyPath
            && Password == other.Password
            && Subject == other.Subject
            && Store == other.Store
            && Location == other.Location
            && (AllowInvalid ?? false) == (other.AllowInvalid ?? false);
    }

    public static bool operator ==(CertificateOptions? left, CertificateOptions? right)
    {
        return left is null
            ? right is null
            : left.Equals(right);
    }

    public static bool operator !=(CertificateOptions? left, CertificateOptions? right)
    {
        return !(left == right);
    }
}
