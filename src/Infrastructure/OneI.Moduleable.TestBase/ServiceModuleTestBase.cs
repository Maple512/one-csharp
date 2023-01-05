namespace OneI.Moduleable;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public abstract class ServiceModuleTestBase<TStartup>
    where TStartup : class
{
    protected ServiceModuleTestBase()
    {
        var services = new ServiceCollection();

        ConfigureServices(services);

        var configurationBuilder = new ConfigurationBuilder();

        ConfigureConfiguration(configurationBuilder);

        Configuration = configurationBuilder.Build();

        services.ConfigureServices<TStartup>(Configuration);

        ServiceProvider = services.BuildServiceProvider();

        _ = ServiceProvider.ConfigureAsync().AsTask().GetAwaiter().GetResult();
    }

    protected IServiceProvider ServiceProvider { get; }

    protected IConfiguration Configuration { get; }

    protected ILogger Logger => ServiceProvider.GetRequiredService<ILogger<TStartup>>();

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(builder =>
         {
             builder.AddDebug();
         });
    }

    protected virtual void ConfigureConfiguration(IConfigurationBuilder builder)
    {
    }

    protected IEnumerable<T> GetServices<T>()
    {
        return ServiceProvider.GetServices<T>();
    }

    protected T? GetService<T>()
    {
        return ServiceProvider.GetService<T>();
    }

    protected T GetRequiredService<T>()
        where T : notnull
    {
        return ServiceProvider.GetRequiredService<T>();
    }
}
