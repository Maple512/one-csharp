namespace OneI.Applicationable;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public interface IApplicationBuilder
{
    IApplicationEnvironment ResolverEnvironment(IConfigurationRoot configuration);

    IApplication Build(IConfigurationRoot configuration, IServiceCollection services);
}
