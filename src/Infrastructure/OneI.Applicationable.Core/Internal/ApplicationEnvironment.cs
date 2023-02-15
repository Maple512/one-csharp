namespace OneI.Applicationable.Internal;

using Microsoft.Extensions.FileProviders;

internal class ApplicationEnvironment : IApplicationEnvironment
{
    public ApplicationEnvironment(ApplicationOptions options)
    {
        EnvironmentName = options.EnvironmentName ?? string.Empty;

        ApplicationName = options.ApplicationName ?? string.Empty;

        var root = options.RootPath ?? string.Empty;
        var basePath = AppContext.BaseDirectory;

        if(root is not { Length: > 0 })
        {
            RootPath = basePath;
        }
        else if(Path.IsPathRooted(root))
        {
            RootPath = root;
        }
        else
        {
            RootPath = Path.Combine(basePath, root);
        }

        FileProvider = new PhysicalFileProvider(RootPath);
    }

    public string EnvironmentName { get; }

    public string ApplicationName { get; }

    public string RootPath { get; }

    public IFileProvider FileProvider { get; }
}
