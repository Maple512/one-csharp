namespace OneI.Applicationable.Internal;

using Microsoft.Extensions.FileProviders;

internal class ApplicationEnvironment : IApplicationEnvironment
{
    public ApplicationEnvironment(
        string environmentName,
        string applicationName,
        string rootPath, 
        IFileProvider? fileProvider)
    {
        EnvironmentName = environmentName;
        ApplicationName = applicationName;
        RootPath = rootPath;
        FileProvider = fileProvider;
    }

    public string EnvironmentName { get; } 

    public string ApplicationName { get; } 

    public string RootPath { get; }

    public IFileProvider? FileProvider { get; }
}
