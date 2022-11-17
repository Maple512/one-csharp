namespace Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OneI.Applicationable;
using OneI.Moduleable;

public static class ServiceCollectionModuleBuilderExtensions
{
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
