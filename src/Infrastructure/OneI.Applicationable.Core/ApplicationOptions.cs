namespace OneI.Applicationable;

using Microsoft.Extensions.Configuration;

public class ApplicationOptions
{
    public string[]? Arguments { get; set; }

    public string? EnvironmentName { get; set; }

    public string? ApplicationName { get; set; }

    public string? RootPath { get; set; }

    public IConfiguration? Configuration { get; set; }

    public TimeSpan ShutdownTimeout { get; set; } = TimeSpan.FromSeconds(30);

    public BackgroundServiceExceptionBehavior BackgroundServiceExceptionBehavior { get; set; } = BackgroundServiceExceptionBehavior.Stop;

    internal void LoadConfiguration()
    {
        if(Configuration is not null)
        {
            EnvironmentName ??= Configuration[ApplicationConstants.Configurations.EnvironmentName];

            ApplicationName ??= Configuration[ApplicationConstants.Configurations.ApplicationName];

            RootPath ??= Configuration[ApplicationConstants.Configurations.RootPath];
        }
    }
}
