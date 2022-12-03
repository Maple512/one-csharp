namespace OneI.Applicationable;

using System;
using static OneI.Applicationable.ApplicationDefinition;

public static class ApplicationEnvironmentExtensions
{
    public static bool IsDevelopment(this IApplicationEnvironment hostEnvironment) => hostEnvironment.IsEnvironment(Environments.Development);

    public static bool IsStaging(this IApplicationEnvironment hostEnvironment) => hostEnvironment.IsEnvironment(Environments.Staging);

    public static bool IsProduction(this IApplicationEnvironment hostEnvironment) => hostEnvironment.IsEnvironment(Environments.Production);

    public static bool IsEnvironment(
        this IApplicationEnvironment hostEnvironment,
        string environmentName)
    {
        return string.Equals(
            hostEnvironment.EnvironmentName,
            environmentName,
            StringComparison.OrdinalIgnoreCase);
    }
}
