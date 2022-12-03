namespace OneI.Moduleable;

using System;

public readonly struct ModuleConfigureContext
{
    public ModuleConfigureContext(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    public IServiceProvider ServiceProvider { get; }

    public override int GetHashCode() => HashCode.Combine(ServiceProvider);
}
