namespace OneI.Hostable;

using static HostableAbstractionsConstants;

public static class HostEnvironmentAbstractionsExtensions
{
    public static bool IsDevelopment(this IHostEnvironment hostEnvironment)
    {
        return hostEnvironment.IsEnvironment(Environments.Development);
    }

    public static bool IsStaging(this IHostEnvironment hostEnvironment)
    {
        return hostEnvironment.IsEnvironment(Environments.Staging);
    }

    public static bool IsProduction(this IHostEnvironment hostEnvironment)
    {
        return hostEnvironment.IsEnvironment(Environments.Production);
    }

    public static bool IsEnvironment(
        this IHostEnvironment hostEnvironment,
        string environmentName)
    {
        if(environmentName is null or { Length: 0 })
        {
            throw new ArgumentNullException(nameof(environmentName));
        }

        return environmentName.AsSpan()
            .Equals(hostEnvironment.EnvironmentName, StringComparison.OrdinalIgnoreCase);
    }
}
