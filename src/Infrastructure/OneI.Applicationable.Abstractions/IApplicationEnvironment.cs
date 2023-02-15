namespace OneI.Applicationable;

using Microsoft.Extensions.FileProviders;

public interface IApplicationEnvironment
{
    string EnvironmentName { get; }

    string ApplicationName { get; }

    string RootPath { get; }

    IFileProvider FileProvider { get; }
}
