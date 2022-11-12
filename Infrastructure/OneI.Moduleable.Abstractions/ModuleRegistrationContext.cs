namespace OneI.Moduleable;

using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 模块注入上下文
/// </summary>
public readonly struct ModuleRegistrationContext
{
    public ModuleRegistrationContext(IServiceCollection services, Assembly assembly)
    {
        Services = services;
        Assembly = assembly;
    }

    public IServiceCollection Services { get; }

    public Assembly Assembly { get; }

    public override int GetHashCode() => HashCode.Combine(Services, Assembly);

    public override string ToString() => $"{nameof(ModuleRegistrationContext)} Assembly: {Assembly.FullName}";
}
