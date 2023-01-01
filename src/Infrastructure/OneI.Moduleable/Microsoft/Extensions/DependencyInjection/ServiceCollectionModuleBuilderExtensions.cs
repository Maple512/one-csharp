namespace Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OneI.Applicationable;
using OneI.Moduleable;

/// <summary>
/// The service collection module builder extensions.
/// </summary>
public static class ServiceCollectionModuleBuilderExtensions
{
    /// <summary>
    /// Configures the services.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="environment">The environment.</param>
    /// <param name="logger">The logger.</param>
    public static void ConfigureServices<TStartup>(
        this IServiceCollection services,
        IConfigurationRoot configuration,
        IApplicationEnvironment? environment = null,
        ILogger? logger = null)
        where TStartup : class
    {
        ModuleBuilder.ConfigureServices<TStartup>(
           configuration,
           services,
           environment,
           logger);
    }
}
