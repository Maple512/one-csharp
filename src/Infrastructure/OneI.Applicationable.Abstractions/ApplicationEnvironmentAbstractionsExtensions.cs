namespace OneI.Applicationable;

using static ApplicationConstants;

public static class ApplicationEnvironmentAbstractionsExtensions
{
    public static bool IsDevelopment(this IApplicationEnvironment hostEnvironment)
    {
        return hostEnvironment.IsEnvironment(Environments.Development);
    }

    public static bool IsStaging(this IApplicationEnvironment hostEnvironment)
    {
        return hostEnvironment.IsEnvironment(Environments.Staging);
    }

    public static bool IsProduction(this IApplicationEnvironment hostEnvironment)
    {
        return hostEnvironment.IsEnvironment(Environments.Production);
    }

    public static bool IsEnvironment(this IApplicationEnvironment hostEnvironment, string environmentName)
    {
        if(environmentName is not { Length: > 0 })
        {
            ThrowHelper.ThrowArgumentNullException(nameof(environmentName));
        }

        return environmentName.AsSpan().Equals(hostEnvironment.EnvironmentName, StringComparison.OrdinalIgnoreCase);
    }
}
