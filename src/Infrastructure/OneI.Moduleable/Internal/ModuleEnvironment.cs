namespace OneI.Moduleable.Internal;

using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using OneI.Applicationable;

/// <summary>
/// The module environment.
/// </summary>
[Serializable]
internal class ModuleEnvironment : IApplicationEnvironment
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
    internal static IApplicationEnvironment Resolve(IConfiguration configuration)
    {
        var applicationName = configuration[ApplicationDefinition.ConfigurationKeys.ApplicationNameKey]!;

        if(applicationName.IsNullOrWhiteSpace())
        {
            applicationName = Assembly.GetEntryAssembly()?.GetName().Name!;
        }

        return new ModuleEnvironment()
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
