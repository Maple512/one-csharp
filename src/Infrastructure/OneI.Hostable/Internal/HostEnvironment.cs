namespace OneI.Hostable.Internal;

using Microsoft.Extensions.FileProviders;

public class HostEnvironment : IHostEnvironment
{
    public string EnvironmentName { get; set; } = string.Empty;

    public string ApplicationName { get; set; } = string.Empty;

    public string RootPath { get; set; } = string.Empty;

    public IFileProvider? FileProvider { get; set; }
}
