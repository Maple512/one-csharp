namespace OneI.Moduleable;

using System;

public readonly struct ModuleConfigureContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleConfigureContext"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public ModuleConfigureContext(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    /// <summary>
    /// Gets the service provider.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets the hash code.
    /// </summary>
    /// <returns>An int.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(ServiceProvider);
    }
}
