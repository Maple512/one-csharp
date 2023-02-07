namespace OneI.Hostable;

using System.Reflection;
using Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static HostableAbstractionsConstants;

public class HostBuilder : IHostBuilder
{
    private const string HostBuildingDiagnosticListenerName = "Microsoft.Extensions.Hosting";
    private const string HostBuildingEventName = "HostBuilding";
    private const string HostBuiltEventName = "HostBuilt";

    private readonly List<Action<IConfigurationBuilder>> _configureHostConfigActions = new();
    private readonly List<Action<HostBuilderContext, IConfigurationBuilder>> _configureAppConfigActions = new();
    private readonly List<Action<HostBuilderContext, IServiceCollection>> _configureServicesActions = new();
    private readonly List<IConfigureContainerAdapter> _configureContainerActions = new();
    private IServiceFactoryAdapter _serviceProviderFactory;
    private bool _hostBuilt;
    private IConfiguration? _hostConfiguration;
    private IConfiguration? _appConfiguration;
    private HostBuilderContext? _hostBuilderContext;
    private HostEnvironment? _hostingEnvironment;
    private IServiceProvider? _appServices;
    private PhysicalFileProvider? _defaultProvider;

    public HostBuilder()
    {
        _serviceProviderFactory = new ServiceFactoryAdapter<IServiceCollection>(new DefaultServiceProviderFactory());
    }

    /// <summary>
    /// Set up the configuration for the builder itself. This will be used to initialize the <see cref="IHostEnvironment"/>
    /// for use later in the build process. This can be called multiple times and the results will be additive.
    /// </summary>
    /// <param name="configureDelegate">The delegate for configuring the <see cref="IConfigurationBuilder"/> that will be used
    /// to construct the <see cref="IConfiguration"/> for the host.</param>
    /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
    public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
    {
        _configureHostConfigActions.Add(configureDelegate);
        return this;
    }

    /// <summary>
    /// Sets up the configuration for the remainder of the build process and application. This can be called multiple times and
    /// the results will be additive. The results will be available at <see cref="HostBuilderContext.Configuration"/> for
    /// subsequent operations, as well as in <see cref="IHost.Services"/>.
    /// </summary>
    /// <param name="configureDelegate">The delegate for configuring the <see cref="IConfigurationBuilder"/> that will be used
    /// to construct the <see cref="IConfiguration"/> for the host.</param>
    /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
    public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
    {
        _configureAppConfigActions.Add(configureDelegate);
        return this;
    }

    /// <summary>
    /// Adds services to the container. This can be called multiple times and the results will be additive.
    /// </summary>
    /// <param name="configureDelegate">The delegate for configuring the <see cref="IConfigurationBuilder"/> that will be used
    /// to construct the <see cref="IConfiguration"/> for the host.</param>
    /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
    public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
    {
        _configureServicesActions.Add(configureDelegate);
        return this;
    }

    /// <summary>
    /// Overrides the factory used to create the service provider.
    /// </summary>
    /// <typeparam name="TContainerBuilder">The type of the builder to create.</typeparam>
    /// <param name="factory">A factory used for creating service providers.</param>
    /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) where TContainerBuilder : notnull
    {
        _serviceProviderFactory = new ServiceFactoryAdapter<TContainerBuilder>(factory);
        return this;
    }

    /// <summary>
    /// Overrides the factory used to create the service provider.
    /// </summary>
    /// <param name="factory">A factory used for creating service providers.</param>
    /// <typeparam name="TContainerBuilder">The type of the builder to create.</typeparam>
    /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory) where TContainerBuilder : notnull
    {
        _serviceProviderFactory = new ServiceFactoryAdapter<TContainerBuilder>(() => _hostBuilderContext!, factory);
        return this;
    }

    /// <summary>
    /// Enables configuring the instantiated dependency container. This can be called multiple times and
    /// the results will be additive.
    /// </summary>
    /// <typeparam name="TContainerBuilder">The type of the builder to create.</typeparam>
    /// <param name="configureDelegate">The delegate for configuring the <see cref="IConfigurationBuilder"/> that will be used
    /// to construct the <see cref="IConfiguration"/> for the host.</param>
    /// <returns>The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
    public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
    {
        _configureContainerActions.Add(new ConfigureContainerAdapter<TContainerBuilder>(configureDelegate));
        return this;
    }

    /// <summary>
    /// Run the given actions to initialize the host. This can only be called once.
    /// </summary>
    /// <returns>An initialized <see cref="IHost"/></returns>
    /// <remarks>Adds basic services to the host such as application lifetime, host environment, and logging.</remarks>
    public IHost Build()
    {
        if(_hostBuilt)
        {
            throw new InvalidOperationException($"The {nameof(Build)} method can only called once.");
        }

        _hostBuilt = true;

        // REVIEW: If we want to raise more events outside of these calls then we will need to
        // stash this in a field.
        using var diagnosticListener = LogHostBuilding(this);

        InitializeHostConfiguration();
        InitializeHostingEnvironment();
        InitializeHostBuilderContext();
        InitializeAppConfiguration();
        InitializeServiceProvider();

        return ResolveHost(_appServices, diagnosticListener);
    }

    private static DiagnosticListener LogHostBuilding(IHostBuilder hostBuilder)
    {
        var diagnosticListener = new DiagnosticListener(HostBuildingDiagnosticListenerName);

        if(diagnosticListener.IsEnabled() && diagnosticListener.IsEnabled(HostBuildingEventName))
        {
            Write(diagnosticListener, HostBuildingEventName, hostBuilder);
        }

        return diagnosticListener;
    }

    internal static DiagnosticListener LogHostBuilding(HostApplicationBuilder hostApplicationBuilder)
    {
        var diagnosticListener = new DiagnosticListener(HostBuildingDiagnosticListenerName);

        if(diagnosticListener.IsEnabled() && diagnosticListener.IsEnabled(HostBuildingEventName))
        {
            Write(diagnosticListener, HostBuildingEventName, hostApplicationBuilder.AsHostBuilder());
        }

        return diagnosticListener;
    }

    // Remove when https://github.com/dotnet/runtime/pull/78532 is merged and consumed by the used SDK.
#if NET7_0
    [UnconditionalSuppressMessage("AOT", "IL3050:RequiresDynamicCode",
        Justification = "DiagnosticSource is used here to pass objects in-memory to code using HostFactoryResolver. This won't require creating new generic types.")]
#endif
    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern",
        Justification = "The values being passed into Write are being consumed by the application already.")]
    private static void Write<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(
        DiagnosticSource diagnosticSource,
        string name,
        T value)
    {
        diagnosticSource.Write(name, value);
    }

    [MemberNotNull(nameof(_hostConfiguration))]
    private void InitializeHostConfiguration()
    {
        var configBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(); // Make sure there's some default storage since there are no default providers

        foreach(var buildAction in _configureHostConfigActions)
        {
            buildAction(configBuilder);
        }

        _hostConfiguration = configBuilder.Build();
    }

    [MemberNotNull(nameof(_defaultProvider))]
    [MemberNotNull(nameof(_hostingEnvironment))]
    private void InitializeHostingEnvironment()
    {
        (_hostingEnvironment, _defaultProvider) = CreateHostingEnvironment(_hostConfiguration!); // TODO-NULLABLE: https://github.com/dotnet/csharplang/discussions/5778. The same pattern exists below as well.
    }

    internal static (HostEnvironment, PhysicalFileProvider) CreateHostingEnvironment(IConfiguration hostConfiguration)
    {
        var hostingEnvironment = new HostEnvironment
        {
            EnvironmentName = hostConfiguration[Configuration.Environment] ?? Environments.Production,
            RootPath = ResolveContentRootPath(hostConfiguration[Configuration.RootPath], AppContext.BaseDirectory),
        };

        var applicationName = hostConfiguration[Configuration.Application];
        if(string.IsNullOrEmpty(applicationName))
        {
            // Note GetEntryAssembly returns null for the net4x console test runner.
            applicationName = Assembly.GetEntryAssembly()?.GetName().Name;
        }

        if(applicationName is not null)
        {
            hostingEnvironment.ApplicationName = applicationName;
        }

        var physicalFileProvider = new PhysicalFileProvider(hostingEnvironment.RootPath);
        hostingEnvironment.FileProvider = physicalFileProvider;

        return (hostingEnvironment, physicalFileProvider);
    }

    internal static string ResolveContentRootPath(string? contentRootPath, string basePath)
    {
        if(string.IsNullOrEmpty(contentRootPath))
        {
            return basePath;
        }

        if(Path.IsPathRooted(contentRootPath))
        {
            return contentRootPath;
        }

        return Path.Combine(Path.GetFullPath(basePath), contentRootPath);
    }

    [MemberNotNull(nameof(_hostBuilderContext))]
    private void InitializeHostBuilderContext()
    {
        _hostBuilderContext = new(_hostingEnvironment!, _hostConfiguration!);
    }

    [MemberNotNull(nameof(_appConfiguration))]
    private void InitializeAppConfiguration()
    {
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(_hostingEnvironment!.RootPath)
            .AddConfiguration(_hostConfiguration!, shouldDisposeConfiguration: true);

        foreach(var buildAction in _configureAppConfigActions)
        {
            buildAction(_hostBuilderContext!, configBuilder);
        }

        _appConfiguration = configBuilder.Build();
        _hostBuilderContext!.Configuration = _appConfiguration;
    }

    [MemberNotNull(nameof(_appServices))]
    internal static void PopulateServiceCollection(
        IServiceCollection services,
        HostBuilderContext hostBuilderContext,
        HostEnvironment hostingEnvironment,
        PhysicalFileProvider defaultFileProvider,
        IConfiguration appConfiguration,
        Func<IServiceProvider> serviceProviderGetter)
    {
        _ = services.AddSingleton<IHostEnvironment>(hostingEnvironment);
        _ = services.AddSingleton(hostBuilderContext);
        // register configuration as factory to make it dispose with the service provider
        _ = services.AddSingleton(_ => appConfiguration);
        _ = services.AddSingleton<IHostApplicationLifetime, HostApplicationLifetime>();

        AddLifetime(services);

        _ = services.AddSingleton<IHost>(_ =>
        {
            // We use serviceProviderGetter() instead of the _ parameter because these can be different given a custom IServiceProviderFactory.
            // We want the host to always dispose the IServiceProvider returned by the IServiceProviderFactory.
            // https://github.com/dotnet/runtime/issues/36060
            var appServices = serviceProviderGetter();

            return new Host(
                appServices.GetRequiredService<ILogger<Host>>(),
                appServices.GetRequiredService<IHostLifetime>(),
                appServices.GetRequiredService<IHostApplicationLifetime>(),
                appServices.GetRequiredService<IOptions<HostOptions>>(),
                hostingEnvironment,
                appServices,
                defaultFileProvider);
        });
        _ = services.AddOptions().Configure<HostOptions>(options => { options.Initialize(hostBuilderContext.Configuration); });
        _ = services.AddLogging();
    }

    [MemberNotNull(nameof(_appServices))]
    private void InitializeServiceProvider()
    {
        var services = new ServiceCollection();

        PopulateServiceCollection(
            services,
            _hostBuilderContext!,
            _hostingEnvironment!,
            _defaultProvider!,
            _appConfiguration!,
            () => _appServices!);

        foreach(var configureServicesAction in _configureServicesActions)
        {
            configureServicesAction(_hostBuilderContext!, services);
        }

        var containerBuilder = _serviceProviderFactory.CreateBuilder(services);

        foreach(var containerAction in _configureContainerActions)
        {
            containerAction.ConfigureContainer(_hostBuilderContext!, containerBuilder);
        }

        _appServices = _serviceProviderFactory.CreateServiceProvider(containerBuilder);
    }

    internal static IHost ResolveHost(IServiceProvider serviceProvider, DiagnosticListener diagnosticListener)
    {
        if(serviceProvider is null)
        {
            throw new InvalidOperationException("The IServiceProviderFactory returned a null IServiceProvider");
        }

        // resolve configuration explicitly once to mark it as resolved within the
        // service provider, ensuring it will be properly disposed with the provider
        _ = serviceProvider.GetService<IConfiguration>();

        var host = serviceProvider.GetRequiredService<IHost>();

        if(diagnosticListener.IsEnabled() && diagnosticListener.IsEnabled(HostBuiltEventName))
        {
            Write(diagnosticListener, HostBuiltEventName, host);
        }

        return host;
    }

    private static void AddLifetime(IServiceCollection services)
    {
        if(!OperatingSystem.IsAndroid()
            && !OperatingSystem.IsBrowser()
            && !OperatingSystem.IsIOS()
            && !OperatingSystem.IsTvOS())
        {
            _ = services.AddSingleton<IHostLifetime, ConsoleLifetime>();
        }
        else
        {
            _ = services.AddSingleton<IHostLifetime, NullHostLifetime>();
        }
    }
}
