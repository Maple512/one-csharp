namespace OneI.Moduleable;

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public readonly ref struct ModuleConfigureServiceContext
{
    public ModuleConfigureServiceContext(
        IServiceCollection services,
        IConfiguration configuration)
    {
        Services = services;
        Configuration = configuration;
    }

    public IServiceCollection Services { get; }

    public IConfiguration Configuration { get; }

    public override int GetHashCode() => HashCode.Combine(Services, Configuration);
}
