namespace OneI.Moduleable;

using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 模块注入上下文
/// </summary>
public readonly struct ModuleRegistrationContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleRegistrationContext"/> class.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="assembly">The assembly.</param>
    public ModuleRegistrationContext(IServiceCollection services, Assembly assembly)
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
        return $"{nameof(ModuleRegistrationContext)} Assembly: {Assembly.FullName}";
    }
}
