namespace OneI.Moduleable;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneI.Applicationable;

public readonly struct ModuleConfigureServiceContext
{
    public ModuleConfigureServiceContext(
        IServiceCollection services,
        IConfiguration configuration,
        IApplicationEnvironment environment)
    {
        Services = services;
        Configuration = configuration;
        Environment = environment;
    }

    public IServiceCollection Services { get; }

    public IConfiguration Configuration { get; }

    public IApplicationEnvironment Environment { get; }

    public override int GetHashCode()
    {
        return HashCode.Combine(Services, Configuration);
    }
}
