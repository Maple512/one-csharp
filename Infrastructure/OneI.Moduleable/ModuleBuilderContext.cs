namespace OneI.Moduleable;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneI.Applicationable;

public class ModuleBuilderContext 
{
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

    public Type StartupType { get; }

    public ILogger? Logger { get; }

    public IServiceCollection Services { get; }

    public IConfiguration Configuration { get; }

    public IApplicationEnvironment Environment { get; }
}
