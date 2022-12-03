namespace OneI.Moduleable.Infrastructure;

using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

public readonly ref struct AssemblyScanContext
{
    public AssemblyScanContext(IServiceCollection services, Assembly assembly)
    {
        Services = services;
        Assembly = assembly;
    }

    public IServiceCollection Services { get; }

    public Assembly Assembly { get; }

    public override int GetHashCode() => HashCode.Combine(Services, Assembly);

    public override string ToString() => $"{nameof(AssemblyScanContext)} Assembly: {Assembly.FullName}";
}
