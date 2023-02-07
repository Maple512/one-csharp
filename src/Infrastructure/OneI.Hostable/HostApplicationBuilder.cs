namespace OneI.Hostable;

using Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class HostApplicationBuilder
{
    private readonly HostBuilderContext _hostBuilderContext;
    private readonly ServiceCollection _serviceCollection = new();

    private Func<IServiceProvider> _createServiceProvider;
    private Action<object> _configureContainer = _ => { };
    private HostBuilderAdapter? _hostBuilderAdapter;

    private IServiceProvider? _appServices;
    private bool _hostBuilt;

    /// <summary>
    /// Initializes a new instance of the <see cref="HostApplicationBuilder"/> class with preconfigured defaults.
    /// </summary>
    /// <remarks>
    ///   The following defaults are applied to the returned <see cref="HostApplicationBuilder"/>:
    ///   <list type="bullet">
    ///     <item><description>set the <see cref="IHostEnvironment.RootPath"/> to the result of <see cref="Directory.GetCurrentDirectory()"/></description></item>
    ///     <item><description>load host <see cref="IConfiguration"/> from "DOTNET_" prefixed environment variables</description></item>
    ///     <item><description>load host <see cref="IConfiguration"/> from supplied command line args</description></item>
    ///     <item><description>load app <see cref="IConfiguration"/> from 'appsettings.json' and 'appsettings.[<see cref="IHostEnvironment.EnvironmentName"/>].json'</description></item>
    ///     <item><description>load app <see cref="IConfiguration"/> from User Secrets when <see cref="IHostEnvironment.EnvironmentName"/> is 'Development' using the entry assembly</description></item>
    ///     <item><description>load app <see cref="IConfiguration"/> from environment variables</description></item>
    ///     <item><description>load app <see cref="IConfiguration"/> from supplied command line args</description></item>
    ///     <item><description>configure the <see cref="ILoggerFactory"/> to log to the console, debug, and event source output</description></item>
    ///     <item><description>enables scope validation on the dependency injection container when <see cref="IHostEnvironment.EnvironmentName"/> is 'Development'</description></item>
    ///   </list>
    /// </remarks>
    public HostApplicationBuilder()
        : this(args: null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostApplicationBuilder"/> class with preconfigured defaults.
    /// </summary>
    /// <remarks>
    ///   The following defaults are applied to the returned <see cref="HostApplicationBuilder"/>:
    ///   <list type="bullet">
    ///     <item><description>set the <see cref="IHostEnvironment.RootPath"/> to the result of <see cref="Directory.GetCurrentDirectory()"/></description></item>
    ///     <item><description>load host <see cref="IConfiguration"/> from "DOTNET_" prefixed environment variables</description></item>
    ///     <item><description>load host <see cref="IConfiguration"/> from supplied command line args</description></item>
    ///     <item><description>load app <see cref="IConfiguration"/> from 'appsettings.json' and 'appsettings.[<see cref="IHostEnvironment.EnvironmentName"/>].json'</description></item>
    ///     <item><description>load app <see cref="IConfiguration"/> from User Secrets when <see cref="IHostEnvironment.EnvironmentName"/> is 'Development' using the entry assembly</description></item>
    ///     <item><description>load app <see cref="IConfiguration"/> from environment variables</description></item>
    ///     <item><description>load app <see cref="IConfiguration"/> from supplied command line args</description></item>
    ///     <item><description>configure the <see cref="ILoggerFactory"/> to log to the console, debug, and event source output</description></item>
    ///     <item><description>enables scope validation on the dependency injection container when <see cref="IHostEnvironment.EnvironmentName"/> is 'Development'</description></item>
    ///   </list>
    /// </remarks>
    /// <param name="args">The command line args.</param>
    public HostApplicationBuilder(string[]? args)
        : this(new HostApplicationBuilderSettings { Args = args })
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostApplicationBuilder"/>.
    /// </summary>
    /// <param name="settings">Settings controlling initial configuration and whether default settings should be used.</param>
    public HostApplicationBuilder(HostApplicationBuilderSettings? settings)
    {
        settings ??= new HostApplicationBuilderSettings();
        Configuration = settings.Configuration ?? new ConfigurationManager();

        if(!settings.DisableDefaults)
        {
            if(settings.RootPath is null && Configuration[HostableAbstractionsConstants.Configuration.RootPath] is null)
            {
                HostingHostBuilderExtensions.SetDefaultContentRoot(Configuration);
            }

            HostingHostBuilderExtensions.AddDefaultHostConfigurationSources(Configuration, settings.Args);
        }

        // HostApplicationBuilderSettings override all other config sources.
        List<KeyValuePair<string, string?>>? optionList = null;
        if(settings.ApplicationName is not null)
        {
            optionList ??= new List<KeyValuePair<string, string?>>();
            optionList.Add(new KeyValuePair<string, string?>(HostableAbstractionsConstants.Configuration.Application, settings.ApplicationName));
        }

        if(settings.EnvironmentName is not null)
        {
            optionList ??= new List<KeyValuePair<string, string?>>();
            optionList.Add(new KeyValuePair<string, string?>(HostableAbstractionsConstants.Configuration.Environment, settings.EnvironmentName));
        }

        if(settings.RootPath is not null)
        {
            optionList ??= new List<KeyValuePair<string, string?>>();
            optionList.Add(new KeyValuePair<string, string?>(HostableAbstractionsConstants.Configuration.RootPath, settings.RootPath));
        }

        if(optionList is not null)
        {
            _ = Configuration.AddInMemoryCollection(optionList);
        }

        (var hostingEnvironment, var physicalFileProvider) = HostBuilder.CreateHostingEnvironment(Configuration);

        _ = Configuration.SetFileProvider(physicalFileProvider);

        _hostBuilderContext = new HostBuilderContext(hostingEnvironment, Configuration);

        Environment = hostingEnvironment;

        HostBuilder.PopulateServiceCollection(
            Services,
            _hostBuilderContext,
            hostingEnvironment,
            physicalFileProvider,
            Configuration,
            () => _appServices!);

        Logging = new LoggingBuilder(Services);

        ServiceProviderOptions? serviceProviderOptions = null;

        if(!settings.DisableDefaults)
        {
            HostingHostBuilderExtensions.ApplyDefaultAppConfiguration(_hostBuilderContext, Configuration, settings.Args);
            HostingHostBuilderExtensions.AddDefaultServices(_hostBuilderContext, Services);
            serviceProviderOptions = HostingHostBuilderExtensions.CreateDefaultServiceProviderOptions(_hostBuilderContext);
        }

        _createServiceProvider = () =>
        {
            // Call _configureContainer in case anyone adds callbacks via HostBuilderAdapter.ConfigureContainer<IServiceCollection>() during build.
            // Otherwise, this no-ops.
            _configureContainer(Services);
            return serviceProviderOptions is null ? Services.BuildServiceProvider() : Services.BuildServiceProvider(serviceProviderOptions);
        };
    }

    /// <summary>
    /// Provides information about the hosting environment an application is running in.
    /// </summary>
    public IHostEnvironment Environment { get; }

    /// <summary>
    /// A collection of services for the application to compose. This is useful for adding user provided or framework provided services.
    /// </summary>
    public ConfigurationManager Configuration { get; }

    /// <summary>
    /// A collection of services for the application to compose. This is useful for adding user provided or framework provided services.
    /// </summary>
    public IServiceCollection Services
    {
        get
        {
            return _serviceCollection;
        }
    }

    /// <summary>
    /// A collection of logging providers for the application to compose. This is useful for adding new logging providers.
    /// </summary>
    public ILoggingBuilder Logging { get; }

    /// <summary>
    /// Registers a <see cref="IServiceProviderFactory{TContainerBuilder}" /> instance to be used to create the <see cref="IServiceProvider" />.
    /// </summary>
    /// <param name="factory">The <see cref="IServiceProviderFactory{TContainerBuilder}" />.</param>
    /// <param name="configure">
    /// A delegate used to configure the <typeparamref T="TContainerBuilder" />. This can be used to configure services using
    /// APIS specific to the <see cref="IServiceProviderFactory{TContainerBuilder}" /> implementation.
    /// </param>
    /// <typeparam name="TContainerBuilder">The type of builder provided by the <see cref="IServiceProviderFactory{TContainerBuilder}" />.</typeparam>
    /// <remarks>
    /// <para>
    /// <see cref="ConfigureContainer{TContainerBuilder}(IServiceProviderFactory{TContainerBuilder}, Action{TContainerBuilder})"/> is called by <see cref="Build"/>
    /// and so the delegate provided by <paramref name="configure"/> will run after all other services have been registered.
    /// </para>
    /// <para>
    /// Multiple calls to <see cref="ConfigureContainer{TContainerBuilder}(IServiceProviderFactory{TContainerBuilder}, Action{TContainerBuilder})"/> will replace
    /// the previously stored <paramref name="factory"/> and <paramref name="configure"/> delegate.
    /// </para>
    /// </remarks>
    public void ConfigureContainer<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory, Action<TContainerBuilder>? configure = null) where TContainerBuilder : notnull
    {
        _createServiceProvider = () =>
        {
            var containerBuilder = factory.CreateBuilder(Services);
            // Call _configureContainer in case anyone adds more callbacks via HostBuilderAdapter.ConfigureContainer<TContainerBuilder>() during build.
            // Otherwise, this is equivalent to configure?.Invoke(containerBuilder).
            _configureContainer(containerBuilder);
            return factory.CreateServiceProvider(containerBuilder);
        };

        // Store _configureContainer separately so it can replaced individually by the HostBuilderAdapter.
        _configureContainer = containerBuilder => configure?.Invoke((TContainerBuilder)containerBuilder);
    }

    /// <summary>
    /// Build the host. This can only be called once.
    /// </summary>
    /// <returns>An initialized <see cref="IHost"/>.</returns>
    public IHost Build()
    {
        if(_hostBuilt)
        {
            throw new InvalidOperationException($"The method {nameof(Build)} can only called once.");
        }

        _hostBuilt = true;

        using var diagnosticListener = HostBuilder.LogHostBuilding(this);
        _hostBuilderAdapter?.ApplyChanges();

        _appServices = _createServiceProvider();

        // Prevent further modification of the service collection now that the provider is built.
        _serviceCollection.MakeReadOnly();

        return HostBuilder.ResolveHost(_appServices, diagnosticListener);
    }

    // Lazily allocate HostBuilderAdapter so the allocations can be avoided if there's nothing observing the events.
    internal IHostBuilder AsHostBuilder() => _hostBuilderAdapter ??= new HostBuilderAdapter(this);

    private sealed class HostBuilderAdapter : IHostBuilder
    {
        private readonly HostApplicationBuilder _hostApplicationBuilder;

        private readonly List<Action<IConfigurationBuilder>> _configureHostConfigActions = new();
        private readonly List<Action<HostBuilderContext, IConfigurationBuilder>> _configureAppConfigActions = new();
        private readonly List<IConfigureContainerAdapter> _configureContainerActions = new();
        private readonly List<Action<HostBuilderContext, IServiceCollection>> _configureServicesActions = new();

        private IServiceFactoryAdapter? _serviceProviderFactory;

        public HostBuilderAdapter(HostApplicationBuilder hostApplicationBuilder)
        {
            _hostApplicationBuilder = hostApplicationBuilder;
        }

        public void ApplyChanges()
        {
            var config = _hostApplicationBuilder.Configuration;

            if(_configureHostConfigActions.Count > 0)
            {
                var previousApplicationName = config[HostableAbstractionsConstants.Configuration.Application];
                var previousEnvironment = config[HostableAbstractionsConstants.Configuration.Environment];
                var previousContentRootConfig = config[HostableAbstractionsConstants.Configuration.RootPath];
                var previousContentRootPath = _hostApplicationBuilder._hostBuilderContext.Environment.RootPath;

                foreach(var configureHostAction in _configureHostConfigActions)
                {
                    configureHostAction(config);
                }

                // Disallow changing any host settings this late in the cycle. The reasoning is that we've already loaded the default configuration
                // and done other things based on environment name, application name or content root.
                if(!string.Equals(previousApplicationName, config[HostableAbstractionsConstants.Configuration.Application], StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotSupportedException($"The application name changed from {previousApplicationName} to {config[HostableAbstractionsConstants.Configuration.Application]}. Changing host configuration is not supported.");
                }

                if(!string.Equals(previousEnvironment, config[HostableAbstractionsConstants.Configuration.Environment], StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotSupportedException($"The environment name changed from {previousEnvironment} to {config[HostableAbstractionsConstants.Configuration.Environment]}. Changing host configuration is not supported.");
                }
                // It's okay if the ConfigureHostConfiguration callbacks either left the config unchanged or set it back to the real ContentRootPath.
                // Setting it to anything else indicates code intends to change the content root via HostFactoryResolver which is unsupported.
                var currentContentRootConfig = config[HostableAbstractionsConstants.Configuration.RootPath];
                if(!string.Equals(previousContentRootConfig, currentContentRootConfig, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(previousContentRootPath, HostBuilder.ResolveContentRootPath(currentContentRootConfig, AppContext.BaseDirectory), StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotSupportedException(
                        $"The content root changed from {previousContentRootConfig} to {currentContentRootConfig} . Changing host configuration is not supported.");
                }
            }

            foreach(var configureAppAction in _configureAppConfigActions)
            {
                configureAppAction(_hostApplicationBuilder._hostBuilderContext, config);
            }

            foreach(var configureServicesAction in _configureServicesActions)
            {
                configureServicesAction(_hostApplicationBuilder._hostBuilderContext, _hostApplicationBuilder.Services);
            }

            if(_configureContainerActions.Count > 0)
            {
                var previousConfigureContainer = _hostApplicationBuilder._configureContainer;

                _hostApplicationBuilder._configureContainer = containerBuilder =>
                {
                    previousConfigureContainer(containerBuilder);

                    foreach(var containerAction in _configureContainerActions)
                    {
                        containerAction.ConfigureContainer(_hostApplicationBuilder._hostBuilderContext, containerBuilder);
                    }
                };
            }

            if(_serviceProviderFactory is not null)
            {
                _hostApplicationBuilder._createServiceProvider = () =>
                {
                    var containerBuilder = _serviceProviderFactory.CreateBuilder(_hostApplicationBuilder.Services);
                    _hostApplicationBuilder._configureContainer(containerBuilder);
                    return _serviceProviderFactory.CreateServiceProvider(containerBuilder);
                };
            }
        }

        public IHost Build() => throw new NotSupportedException();

        public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
        {
            _configureHostConfigActions.Add(configureDelegate);
            return this;
        }

        public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            _configureAppConfigActions.Add(configureDelegate);
            return this;
        }

        public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
        {
            _configureServicesActions.Add(configureDelegate);
            return this;
        }

        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) where TContainerBuilder : notnull
        {
            _serviceProviderFactory = new ServiceFactoryAdapter<TContainerBuilder>(factory);
            return this;

        }

        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory) where TContainerBuilder : notnull
        {
            _serviceProviderFactory = new ServiceFactoryAdapter<TContainerBuilder>(() => _hostApplicationBuilder._hostBuilderContext, factory);
            return this;
        }

        public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
        {
            _configureContainerActions.Add(new ConfigureContainerAdapter<TContainerBuilder>(configureDelegate));
            return this;
        }
    }

    private sealed class LoggingBuilder : ILoggingBuilder
    {
        public LoggingBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
