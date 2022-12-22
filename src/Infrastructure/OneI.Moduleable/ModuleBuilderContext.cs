namespace OneI.Moduleable;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneI.Applicationable;
/// <summary>
/// The module builder context.
/// </summary>

public class ModuleBuilderContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleBuilderContext"/> class.
    /// </summary>
    /// <param name="startupType">The startup type.</param>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="environment">The environment.</param>
    /// <param name="logger">The logger.</param>
    public ModuleBuilderContext(
        Type startupType,
        IServiceCollection services,
        IConfiguration configuration,
        IApplicationEnvironment environment,
        ILogger? logger = null)
    {
        Logger = logger;
        Services = services;
        Configuration = configuration;
        Environment = environment;
        StartupType = startupType;
    }

    /// <summary>
    /// Gets the startup type.
    /// </summary>
    public Type StartupType { get; }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    public ILogger? Logger { get; }

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
}
