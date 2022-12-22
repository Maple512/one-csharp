namespace OneI.Applicationable;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneI.Applicationable.Internal;
/// <summary>
/// The application builder.
/// </summary>

public sealed class ApplicationBuilder : IApplicationBuilder
{
    /// <summary>
    /// The diagnostic listener name.
    /// </summary>
    private const string DiagnosticListenerName = "OneI.Applicationable";

    /// <summary>
    /// The application builder event name.
    /// </summary>
    private const string ApplicationBuilderEventName = "ApplicationBuilding";

    private bool built;

    private readonly IServiceFactoryAdapter _serviceProviderFactory = new ServiceFactoryAdapter<IServiceCollection>(new DefaultServiceProviderFactory());

    //private readonly ApplicationBuilderContext _builderContext;
    private IServiceProvider? _serviceProvider;

    /// <summary>
    /// Builds the.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="services">The services.</param>
    /// <returns>An IApplication.</returns>
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

    /// <summary>
    /// Resolvers the environment.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <returns>An IApplicationEnvironment.</returns>
    public IApplicationEnvironment ResolverEnvironment(IConfigurationRoot configuration)
    {
        return ApplicationEnvironment.Resolver(configuration);
    }

    /// <summary>
    /// Initializes the service provider.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="services">The services.</param>
    private void InitializeServiceProvider(IConfigurationRoot configuration, IServiceCollection services)
    {
        PopulateServiceCollection(
            services,
            configuration,
            () => _serviceProvider);

        var containerBuilder = _serviceProviderFactory.CreateBuilder(services);

        _serviceProvider = _serviceProviderFactory.CreateServiceProvider(containerBuilder);
    }

    /// <summary>
    /// Populates the service collection.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="serviceProviderFactory">The service provider factory.</param>
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

    /// <summary>
    /// Logs the building.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>A DiagnosticListener.</returns>
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

    /// <summary>
    /// Resolvers the.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="diagnosticListener">The diagnostic listener.</param>
    /// <returns>An IApplication.</returns>
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

    /// <summary>
    /// Writes the.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern",
            Justification = "The values being passed into Write are being consumed by the application already.")]
    private static void Write<T>(DiagnosticSource source, string name, T value)
    {
        source.Write(name, value);
    }
}
