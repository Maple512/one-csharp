namespace OneI.Applicationable;

using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;

[Serializable]
public class ApplicationEnvironment : IApplicationEnvironment
{
    public string? EnvironmentName { get; private set; }

    public string ApplicationName { get; private set; }

    public string RootPath { get; private set; }

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
