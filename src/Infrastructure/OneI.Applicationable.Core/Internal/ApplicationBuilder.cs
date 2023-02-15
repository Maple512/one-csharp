namespace OneI.Applicationable.Internal;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneI.Logable;

internal class ApplicationBuilder : IApplicationBuilder
{
    private readonly List<Action<IApplicationBuilderContext, IConfigurationBuilder>> _configurations = new();
    private readonly List<Action<IApplicationBuilderContext, IServiceCollection>> _serviceConfigurations = new();
    private readonly List<Action<IApplicationBuilderContext, ILoggerConfiguration>> _loggerConfigurations = new();
    private IServiceProviderFactoryBuilder? _serviceFactoryBuilder;
    private readonly ApplicationBuilderContext _builderContext;
    private bool _built;
    private readonly ApplicationOptions _options;

    public ApplicationBuilder(ApplicationOptions options)
    {
        options.LoadConfiguration();

        var configurationBulder = new ConfigurationBuilder()
            .AddInMemoryCollection();

        if(options.Configuration is not null)
        {
            _ = configurationBulder.AddConfiguration(options.Configuration);
        }

        _builderContext = new ApplicationBuilderContext(new ApplicationEnvironment(options), configurationBulder.Build());

        _options = options;
    }

    public IApplication Build()
    {
        if(_built)
        {
            throw new InvalidOperationException("Build can only be called once.");
        }

        _built = true;

        using var listener = CreateListener(this);

        InitializeConfiguration();

        var services = new ServiceCollection();

        IntializeLogger(services);

        var serviceProvider = InitializeServiceProvider(services);

        return new Application(
            serviceProvider,
            _builderContext.Configuration,
            _builderContext.Environment,
            serviceProvider.GetRequiredService<IApplicationLifetime>(),
            serviceProvider.GetRequiredService<ILogger<ApplicationBuilder>>(),
            serviceProvider.GetRequiredService<IApplicationHostLifetime>(),
           _options,
           _builderContext.Environment.FileProvider);
    }

    public IApplicationBuilder ConfigureConfiguration(Action<IApplicationBuilderContext, IConfigurationBuilder> configure)
    {
        _configurations.Add(configure);

        return this;
    }

    public IApplicationBuilder ConfigureLogger(Action<IApplicationBuilderContext, ILoggerConfiguration> configure)
    {
        _loggerConfigurations.Add(configure);

        return this;
    }

    public IApplicationBuilder ConfigureServiceProviderFactory<TContainerBuilder>(Action<IApplicationBuilderContext, IServiceProviderFactoryProvider<TContainerBuilder>> configure)
        where TContainerBuilder : notnull
    {
        _serviceFactoryBuilder = new ServiceProviderFactoryBuilder<TContainerBuilder>(configure);

        return this;
    }

    public IApplicationBuilder ConfigureServices(Action<IApplicationBuilderContext, IServiceCollection> configure)
    {
        _serviceConfigurations.Add(configure);

        return this;
    }

    private void InitializeConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(_builderContext.Environment.RootPath)
            .AddConfiguration(_builderContext.Configuration);

        for(var i = 0; i < _configurations.Count; i++)
        {
            _configurations[i].Invoke(_builderContext, builder);
        }

        _builderContext.Configuration = builder.Build();
    }

    private IServiceProvider InitializeServiceProvider(ServiceCollection services)
    {
        _ = services.AddSingleton(_builderContext.Environment);
        _ = services.AddSingleton(_ => _builderContext.Configuration);
        services.AddSingleton<IApplicationLifetime, ApplicationLifetime>();

        AddLifetime(services);

        services.AddOptions();

        for(var i = 0; i < _serviceConfigurations.Count; i++)
        {
            _serviceConfigurations[i].Invoke(_builderContext, services);
        }

        IServiceProvider? provider;
        if(_serviceFactoryBuilder is not null)
        {
            provider = _serviceFactoryBuilder.Build(_builderContext, services);
        }
        else
        {
            provider = new DefaultServiceProviderFactory()
                .CreateServiceProvider(services);
        }

        return provider;
    }

    private ILogger IntializeLogger(ServiceCollection services)
    {
        var builder = new LoggerConfiguration();

        for(var i = 0; i < _loggerConfigurations.Count; i++)
        {
            _loggerConfigurations[i].Invoke(_builderContext, builder);
        }

        var logger = builder.CreateLogger();

        services.AddSingleton(logger);
        services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

        return logger;
    }

    private static DiagnosticListener CreateListener(IApplicationBuilder builder)
    {
        var listener = new DiagnosticListener(ApplicationConstants.Listener.Name);

        if(listener.IsEnabled()
            && listener.IsEnabled(ApplicationConstants.Listener.Events.Building))
        {
            listener.Write(ApplicationConstants.Listener.Events.Building, builder);
        }

        return listener;
    }

    private static void AddLifetime(IServiceCollection services)
    {
        if(!OperatingSystem.IsAndroid() && !OperatingSystem.IsBrowser() && !OperatingSystem.IsIOS() && !OperatingSystem.IsTvOS())
        {
            services.AddSingleton<IApplicationHostLifetime, ConsoleLifetime>();
        }
        else
        {
            services.AddSingleton<IApplicationHostLifetime, NullLifetime>();
        }
    }
}
