namespace OneI.Applicationable;

using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
/// <summary>
/// The application environment.
/// </summary>

[Serializable]
public class ApplicationEnvironment : IApplicationEnvironment
{
    /// <summary>
    /// Gets the environment name.
    /// </summary>
    public string? EnvironmentName { get; private set; }

    /// <summary>
    /// Gets the application name.
    /// </summary>
    public string? ApplicationName { get; private set; }

    /// <summary>
    /// Gets the root path.
    /// </summary>
    public string? RootPath { get; private set; }

    /// <summary>
    /// Resolvers the.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <returns>An IApplicationEnvironment.</returns>
    internal static IApplicationEnvironment Resolver(IConfiguration configuration)
    {
        var applicationName = configuration[ApplicationDefinition.ConfigurationKeys.ApplicationNameKey]!;

        if(applicationName.IsNullOrWhiteSpace())
        {
            applicationName = Assembly.GetEntryAssembly()?.GetName().Name!;
        }

        return new ApplicationEnvironment()
        {
            EnvironmentName = configuration[ApplicationDefinition.ConfigurationKeys.EnvironmentNameKey]!,
            ApplicationName = applicationName,
            RootPath = ResolveContentRootPath(configuration[ApplicationDefinition.ConfigurationKeys.RootPathKey]!, AppContext.BaseDirectory),
        };
    }

    /// <summary>
    /// Resolves the content root path.
    /// </summary>
    /// <param name="contentRootPath">The content root path.</param>
    /// <param name="basePath">The base path.</param>
    /// <returns>A string.</returns>
    private static string ResolveContentRootPath(string? contentRootPath, string basePath)
    {
        if(contentRootPath.IsNullOrEmpty())
        {
            return basePath;
        }

        if(Path.IsPathRooted(contentRootPath))
        {
            return contentRootPath;
        }

        return Path.Combine(Path.GetFullPath(basePath), contentRootPath!);
    }
}
