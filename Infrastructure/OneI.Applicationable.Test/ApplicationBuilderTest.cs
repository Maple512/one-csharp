namespace OneI.Applicationable;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

public class ApplicationBuilderTest
{
    [Fact]
    public async void Application_build()
    {
        var builder = new ApplicationBuilder();

        var configuration = new ConfigurationBuilder()
             .AddJsonFile("appsettings.json")
             .Build();

        var services = new ServiceCollection();

        var environment = configuration.GetSection(ApplicationDefinition.ConfigurationKeys.EnvironmentNameKey);

        services.TryAddSingleton<ApplicationBuilderTest>();

        services.AddLogging(logger =>
        {
            logger.AddConsole().AddDebug();
        });

        var application = new ApplicationBuilder().Build(configuration, services);

        await application.StartAsync();

        await Task.Delay(TimeSpan.FromSeconds(10));

        await application.StopAsync();
    }
}
