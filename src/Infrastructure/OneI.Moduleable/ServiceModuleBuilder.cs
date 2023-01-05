namespace OneI.Moduleable;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneI.Applicationable;
using OneI.Moduleable.Internal;

/// <summary>
/// 表示服务模块构建器
/// </summary>
public static class ServiceModuleBuilder
{
    /// <summary>
    /// Configures the services.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="services">The services.</param>
    /// <param name="environment">The environment.</param>
    /// <param name="logger">The logger.</param>
    public static void ConfigureServices<TStartup>(
        IConfiguration configuration,
        IServiceCollection? services = null,
        IApplicationEnvironment? environment = null,
        ILogger? logger = null)
        where TStartup : class
    {
        var startupType = typeof(TStartup);

        services ??= new ServiceCollection();

        environment ??= ModuleEnvironment.Resolve(configuration);

        ConfigureServices(new(
            startupType,
            services,
            configuration,
            environment,
            logger));
    }

    /// <summary>
    /// Configures the services.
    /// </summary>
    /// <param name="context">The context.</param>
    public static void ConfigureServices(ServiceModuleBuilderContext context)
    {
        context.Services.AddOptions();

        var modules = ModuleHelper.LoadModules(context.StartupType, context.Logger);

        var moduleContext = new ModuleContainer(modules);

        context.Services.AddSingleton<IServiceModuleContainer>(moduleContext);

        ConfigureServices(modules, context);
    }

    /// <summary>
    /// Configures the services.
    /// </summary>
    /// <param name="modules">The modules.</param>
    /// <param name="context">The context.</param>
    private static void ConfigureServices(
        IReadOnlyList<IServiceModuleDescriptor> modules,
        ServiceModuleBuilderContext context)
    {
        // Assembly scan
        foreach(var module in modules)
        {
            var registrationContext = new ServiceModuleRegistrationContext(context.Services, module.Assembly);

            if(module.Module is IServiceModuleInjector injector)
            {
                if(injector.KeepDefault)
                {
                    // default
                    ModuleInjectorTools.RegistrationModule(registrationContext);
                }

                injector.Inject(registrationContext);
            }
            else
            {
                // default
                ModuleInjectorTools.RegistrationModule(registrationContext);
            }
        }

        // Configure services
        var configureServiceContext = new ServiceModuleConfigureServiceContext(
            context.Services,
            context.Configuration,
            context.Environment);

        foreach(var module in modules)
        {
            if(module.Module is IServiceModuleConfigureServices configure)
            {
                configure.ConfigureServices(configureServiceContext);
            }
        }

        foreach(var module in modules)
        {
            if(module.Module is IServiceModulePostConfigureServices postConfigure)
            {
                postConfigure.PostConfigureServices(configureServiceContext);
            }
        }
    }

    /// <summary>
    /// Configures the async.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>A Task.</returns>
    public static async Task<IServiceProvider> ConfigureAsync(IServiceProvider serviceProvider)
    {
        var moduleContext = serviceProvider.GetRequiredService<IServiceModuleContainer>();

        await ConfigureAsync(moduleContext.Modules, serviceProvider);

        return serviceProvider;
    }

    /// <summary>
    /// Configures the async.
    /// </summary>
    /// <param name="modules">The modules.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>A Task.</returns>
    private static async Task ConfigureAsync(IReadOnlyList<IServiceModuleDescriptor> modules, IServiceProvider serviceProvider)
    {
        var context = new ServiceModuleConfigureContext(serviceProvider);

        foreach(var module in modules)
        {
            if(module.Module is IServiceModuleConfigure configure)
            {
                await configure.ConfigureAsync(context);
            }
        }
    }
}
