namespace OneI.Moduleable.Infrastructure;

using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

public readonly struct AssemblyScanContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyScanContext"/> class.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="assembly">The assembly.</param>
    public AssemblyScanContext(IServiceCollection services, Assembly assembly)
    {
        Services = services;
        Assembly = assembly;
    }

    /// <summary>
    /// Gets the services.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Gets the assembly.
    /// </summary>
    public Assembly Assembly { get; }

    /// <summary>
    /// Gets the hash code.
    /// </summary>
    /// <returns>An int.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Services, Assembly);
    }

    /// <summary>
    /// Tos the string.
    /// </summary>
    /// <returns>A string.</returns>
    public override string ToString()
    {
        return $"{nameof(AssemblyScanContext)} Assembly: {Assembly.FullName}";
    }
}
