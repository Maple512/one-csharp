namespace OneI.Applicationable;

using System;
using static OneI.Applicationable.ApplicationDefinition;
/// <summary>
/// The application environment extensions.
/// </summary>

public static class ApplicationEnvironmentExtensions
{
    /// <summary>
    /// Are the development.
    /// </summary>
    /// <param name="hostEnvironment">The host environment.</param>
    /// <returns>A bool.</returns>
    public static bool IsDevelopment(this IApplicationEnvironment hostEnvironment)
    {
        return hostEnvironment.IsEnvironment(Environments.Development);
    }

    /// <summary>
    /// Are the staging.
    /// </summary>
    /// <param name="hostEnvironment">The host environment.</param>
    /// <returns>A bool.</returns>
    public static bool IsStaging(this IApplicationEnvironment hostEnvironment)
    {
        return hostEnvironment.IsEnvironment(Environments.Staging);
    }

    /// <summary>
    /// Are the production.
    /// </summary>
    /// <param name="hostEnvironment">The host environment.</param>
    /// <returns>A bool.</returns>
    public static bool IsProduction(this IApplicationEnvironment hostEnvironment)
    {
        return hostEnvironment.IsEnvironment(Environments.Production);
    }

    /// <summary>
    /// Are the environment.
    /// </summary>
    /// <param name="hostEnvironment">The host environment.</param>
    /// <param name="environmentName">The environment name.</param>
    /// <returns>A bool.</returns>
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
