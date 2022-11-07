namespace Microsoft.Extensions.DependencyInjection;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OneI;
using OneI.Moduleable;

public static class ServiceCollectionModuleContextExtensions
{
    public static IConfiguration ConfigureServices<TStartup>(
        this IServiceCollection services,
        Action<IConfigurationBuilder>? configurationBuilderAction = null,
        Action<string>? logger = null)
    {
        return ModuleFactory.ConfigureServices<TStartup>(
            services,
            configurationBuilderAction,
            logger);
    }

    public static void ConfigureServices<TStartup>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<string>? logger = null)
    {
        _ = CheckTools.NotNull(configuration);

        ModuleFactory.ConfigureServices(
           typeof(TStartup),
           services,
           configuration,
           logger);
    }

    public static void ConfigureServices(
        this IServiceCollection services,
        Type startupType,
        IConfiguration configuration,
        Action<string>? logger = null)
    {
        _ = CheckTools.NotNull(startupType);
        _ = CheckTools.NotNull(configuration);

        ModuleFactory.ConfigureServices(
           startupType,
           services,
           configuration,
           logger);
    }

    public static async ValueTask<IServiceProvider> ConfigureAsync(this IServiceProvider serviceProvider)
    {
        return await ModuleFactory.ConfigureAsync(serviceProvider);
    }
}
