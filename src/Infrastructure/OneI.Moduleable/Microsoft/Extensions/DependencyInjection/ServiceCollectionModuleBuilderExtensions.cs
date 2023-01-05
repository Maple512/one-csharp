namespace Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OneI.Applicationable;
using OneI.Moduleable;

public static class ServiceCollectionModuleBuilderExtensions
{
    public static void ConfigureServices<TStartup>(
        this IServiceCollection services,
        IConfiguration configuration,
        IApplicationEnvironment? environment = null,
        ILogger? logger = null)
        where TStartup : class
    {
        ServiceModuleBuilder.ConfigureServices<TStartup>(
           configuration,
           services,
           environment,
           logger);
    }
}
