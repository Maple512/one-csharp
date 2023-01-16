namespace OneI.Hostable;

using Microsoft.Extensions.FileProviders;

public interface IHostEnvironment
{
    string EnvironmentName { get; }

    string ApplicationName { get; }

    string RootPath { get; }

    /// <summary>
    /// root path file provider
    /// </summary>
    IFileProvider? FileProvider { get; }
}
