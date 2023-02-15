namespace OneI.Applicationable;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneI.Logable;

public interface IApplicationBuilder
{
    IApplicationBuilder ConfigureConfiguration(Action<IApplicationBuilderContext, IConfigurationBuilder> configure);

    IApplicationBuilder ConfigureLogger(Action<IApplicationBuilderContext, ILoggerConfiguration> configure);

    IApplicationBuilder ConfigureServices(Action<IApplicationBuilderContext, IServiceCollection> configure);

    IApplicationBuilder ConfigureServiceProviderFactory<TContainerBuilder>(Action<IApplicationBuilderContext, IServiceProviderFactoryProvider<TContainerBuilder>> configure)
        where TContainerBuilder : notnull;

    IApplication Build();
}
