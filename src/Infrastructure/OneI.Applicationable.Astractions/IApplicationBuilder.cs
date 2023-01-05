namespace OneI.Applicationable;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
/// <summary>
/// The application builder.
/// </summary>

public interface IApplicationBuilder
{
    /// <summary>
    /// Resolvers the environment.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <returns>An IApplicationEnvironment.</returns>
    IApplicationEnvironment ResolveEnvironment(IConfigurationRoot configuration);

    /// <summary>
    /// Builds the.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="services">The services.</param>
    /// <returns>An IApplication.</returns>
    IApplication Build(IConfigurationRoot configuration, IServiceCollection services);
}
