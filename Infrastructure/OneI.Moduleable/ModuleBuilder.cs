namespace OneI.Moduleable;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneI.Applicationable;
using OneI.Moduleable.Infrastructure;
using OneI.Moduleable.Internal;

public static class ModuleBuilder
{
    public static void ConfigureServices<TStartup>(
        IConfigurationRoot configuration,
        IServiceCollection? services = null,
        IApplicationEnvironment? environment = null,
        ILogger? logger = null)
        where TStartup : class
    {
        var startupType = typeof(TStartup);

        services ??= new ServiceCollection();

        environment ??= ModuleEnvironment.Resolver(configuration);

        ConfigureServices(new(
            startupType,
            services,
            configuration,
            environment,
            logger));
    }

    public static void ConfigureServices(ModuleBuilderContext context)
    {
        context.Services.AddOptions();

        var modules = ModuleHelper.LoadModules(context.Services, context.StartupType, context.Logger);

        var moduleContext = new ModuleContainer(modules);

        context.Services.AddSingleton<IModuleContainer>(moduleContext);

        ConfigureServices(modules, context);
    }

    private static void ConfigureServices(
        IReadOnlyList<IModuleDescriptor> modules,
        ModuleBuilderContext context)
    {
        // Assembly scan
        foreach(var module in modules)
        {
            var registrationContext = new ModuleRegistrationContext(context.Services, module.Assembly);

            if(module.Module is IModuleInjector scanner)
            {
                if(scanner.KeepDefault)
                {
                    // default
                    ModuleInjectorTools.RegistrationModule(registrationContext);
                }

                scanner.Scan(registrationContext);
            }
            else
            {
                // default
                ModuleInjectorTools.RegistrationModule(registrationContext);
            }
        }

        // Configure services
        var configureServiceContext = new ModuleConfigureServiceContext(
            context.Services,
            context.Configuration,
            context.Environment);

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

    public static async Task<IServiceProvider> ConfigureAsync(IServiceProvider serviceProvider)
    {
        var moduleContext = serviceProvider.GetRequiredService<IModuleContainer>();

        await ConfigureAsync(moduleContext.Modules, serviceProvider);

        return serviceProvider;
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
