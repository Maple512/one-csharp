namespace OneI.Moduleable;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneI.Moduleable.Infrastructure;

public static class ModuleFactory
{
    public static IConfiguration ConfigureServices<TStartup>(
        IServiceCollection? services = null,
        Action<IConfigurationBuilder>? configurationBuilderAction = null,
        Action<string>? logger = null)
    {
        return ConfigureServices(
            typeof(TStartup),
            services,
            configurationBuilderAction,
            logger);
    }

    public static IConfiguration ConfigureServices(
        Type startupType,
        IServiceCollection? services = null,
        Action<IConfigurationBuilder>? configurationBuilderAction = null,
        Action<string>? logger = null)
    {
        services ??= new ServiceCollection();

        var configurationBuilder = new ConfigurationBuilder();

        configurationBuilderAction ??= static (IConfigurationBuilder c) => { };

        configurationBuilderAction.Invoke(configurationBuilder);

        var configuration = configurationBuilder.Build();

        ConfigureServices(
           startupType,
           services,
           configuration,
           logger);

        return configuration;
    }

    public static void ConfigureServices(
        Type startupType,
        IServiceCollection services,
        IConfiguration configuration,
        Action<string>? logger = null)
    {
        _ = CheckTools.NotNull(startupType);
        _ = CheckTools.NotNull(services);
        _ = CheckTools.NotNull(configuration);

        _ = services.AddOptions();

        var modules = ModuleHelper.LoadModules(services, startupType, logger);

        var moduleContext = new ModuleContainer(modules);

        _ = services.AddSingleton(moduleContext);

        ConfigureServices(modules, services, configuration);
    }

    public static async Task<IServiceProvider> ConfigureAsync(IServiceProvider serviceProvider)
    {
        var moduleContext = serviceProvider.GetRequiredService<ModuleContainer>();

        await ConfigureAsync(moduleContext.Modules, serviceProvider);

        return serviceProvider;
    }

    private static void ConfigureServices(
        IReadOnlyList<IModuleDescriptor> modules,
        IServiceCollection services,
        IConfiguration configuration)
    {
        // Assembly scan
        foreach(var module in modules)
        {
            var context = new ModuleInjectionContext(services, module.Assembly);

            if(module.Module is IModuleInjector scanner)
            {
                if(scanner.KeepDefault)
                {
                    // default
                    ModuleInjectorTools.RegistrationModule(context);
                }

                scanner.Scan(context);
            }
            else
            {
                // default
                ModuleInjectorTools.RegistrationModule(context);
            }
        }

        // Configure services
        var configureServiceContext = new ModuleConfigureServiceContext(services, configuration);

        foreach(var module in modules)
        {
            if(module.Module is IModuleConfigureServices configure)
            {
                configure.ConfigureServices(configureServiceContext);
            }
        }

        foreach(var module in modules)
        {
            if(module.Module is IModulePostConfigureServices postConfigure)
            {
                postConfigure.PostConfigureServices(configureServiceContext);
            }
        }
    }

    private static async Task ConfigureAsync(IReadOnlyList<IModuleDescriptor> modules, IServiceProvider serviceProvider)
    {
        var context = new ModuleConfigureContext(serviceProvider);

        foreach(var module in modules)
        {
            if(module.Module is IModuleConfigure configure)
            {
                await configure.ConfigureAsync(context);
            }
        }
    }
}
