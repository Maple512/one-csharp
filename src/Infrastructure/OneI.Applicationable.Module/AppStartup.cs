namespace OneI.Applicationable;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneI.Moduleable;

public class AppStartup : ServiceModule
{
    public static async Task RunAsync<TStartupModule>(
        Action<IConfigurationBuilder>? configurationBuilderAction = null,
        ILogger? logger = null)
        where TStartupModule : class
    {
        var configurationBuilder = new ConfigurationBuilder()
           .AddInMemoryCollection();

        configurationBuilderAction?.Invoke(configurationBuilder);

        var configuration = configurationBuilder.Build();

        var builder = new ApplicationBuilder();

        var environment = builder.ResolveEnvironment(configuration);

        var services = new ServiceCollection();

        services.ConfigureServices<TStartupModule>(configuration, environment, logger);

        var application = builder.Build(configuration, services);

        await application.ServiceProvider.ConfigureAsync();

        await application.StartAsync();
    }
}
