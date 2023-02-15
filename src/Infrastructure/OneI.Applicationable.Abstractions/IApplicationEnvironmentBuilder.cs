namespace OneI.Applicationable;

using Microsoft.Extensions.FileProviders;

public interface IApplicationEnvironmentBuilder
{
    string EnvironmentName { get; set; }

    string ApplicationName { get; set; }

    string RootPath { get; set; }

    IFileProvider? FileProvider { get; set; }
}
