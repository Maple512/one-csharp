namespace OneI.Applicationable;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneI.Applicationable.Internal;

public sealed class ApplicationBuilder : IApplicationBuilder
{
    private const string DiagnosticListenerName = "OneI.Applicationable";

    private const string ApplicationBuilderEventName = "ApplicationBuilding";

    private bool built;

    private readonly IServiceFactoryAdapter _serviceProviderFactory = new ServiceFactoryAdapter<IServiceCollection>(new DefaultServiceProviderFactory());

    //private readonly ApplicationBuilderContext _builderContext;
    private IServiceProvider _serviceProvider;

    public IApplication Build(IConfigurationRoot configuration, IServiceCollection services)
    {
        if(built)
        {
            throw new InvalidOperationException("Build can only be called once.");
        }

        built = true;

        using var diagnostic = LogBuilding(this);

        InitializeServiceProvider(configuration, services);

        return Resolver(_serviceProvider, diagnostic);
    }

    public IApplicationEnvironment ResolverEnvironment(IConfigurationRoot configuration) => ApplicationEnvironment.Resolver(configuration);

    private void InitializeServiceProvider(IConfigurationRoot configuration, IServiceCollection services)
    {
        PopulateServiceCollection(
            services,
            configuration,
            () => _serviceProvider);

        var containerBuilder = _serviceProviderFactory.CreateBuilder(services);

        _serviceProvider = _serviceProviderFactory.CreateServiceProvider(containerBuilder);
    }

    [MemberNotNull(nameof(_serviceProvider))]
    private static void PopulateServiceCollection(
        IServiceCollection services,
        IConfiguration configuration,
        Func<IServiceProvider> serviceProviderFactory)
    {
        var environment = ApplicationEnvironment.Resolver(configuration);

        services.AddSingleton<IApplicationEnvironment>(environment);

        services.AddSingleton<IConfiguration>(_ => configuration);

        services.AddSingleton<IApplicationLifetimeService, ApplicationLifetimeService>();

        services.AddSingleton<ITerminalService, TerminalService>();

        services.AddSingleton<IApplication>(_ =>
        {
            var serviceProvider = serviceProviderFactory();

            return new Application(serviceProvider);
        });

        services.AddOptions();

        services.AddLogging();
    }

    private static DiagnosticListener LogBuilding(IApplicationBuilder builder)
    {
        var dl = new DiagnosticListener(DiagnosticListenerName);

        if(dl.IsEnabled()
            && dl.IsEnabled(ApplicationBuilderEventName))
        {
            Write(dl, ApplicationBuilderEventName, builder);
        }

        return dl;
    }

    private static IApplication Resolver(IServiceProvider serviceProvider, DiagnosticListener diagnosticListener)
    {
        Check.NotNull(serviceProvider);

        // 显式解析一次，已将其在服务提供程序中标记为已解析，确保该配置由提供程序正确处理
        _ = serviceProvider.GetService<IConfiguration>();

        var application = serviceProvider.GetRequiredService<IApplication>();

        if(diagnosticListener.IsEnabled()
            && diagnosticListener.IsEnabled(ApplicationBuilderEventName))
        {
            Write(diagnosticListener, ApplicationBuilderEventName, application);
        }

        return application;
    }

    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern",
            Justification = "The values being passed into Write are being consumed by the application already.")]
    private static void Write<T>(DiagnosticSource source, string name, T value) => source.Write(name, value);
}
