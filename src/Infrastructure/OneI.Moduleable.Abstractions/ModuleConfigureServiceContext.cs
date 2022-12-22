namespace OneI.Moduleable;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneI.Applicationable;

public readonly struct ModuleConfigureServiceContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleConfigureServiceContext"/> class.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="environment">The environment.</param>
    public ModuleConfigureServiceContext(
        IServiceCollection services,
        IConfiguration configuration,
        IApplicationEnvironment environment)
    {
        Services = services;
        Configuration = configuration;
        Environment = environment;
    }

    /// <summary>
    /// Gets the services.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// Gets the environment.
    /// </summary>
    public IApplicationEnvironment Environment { get; }

    /// <summary>
    /// Gets the hash code.
    /// </summary>
    /// <returns>An int.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Services, Configuration);
    }
}
