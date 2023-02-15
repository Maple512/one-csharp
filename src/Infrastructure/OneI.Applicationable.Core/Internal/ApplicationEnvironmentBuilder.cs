namespace OneI.Applicationable.Internal;

using Microsoft.Extensions.FileProviders;

internal sealed class ApplicationEnvironmentBuilder : IApplicationEnvironmentBuilder
{
    public string EnvironmentName { get; set; } = null!;

    public string ApplicationName { get; set; } = null!;

    public string RootPath { get; set; } = null!;

    public IFileProvider? FileProvider { get; set; }
}
