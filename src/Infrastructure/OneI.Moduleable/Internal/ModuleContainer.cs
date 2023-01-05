namespace OneI.Moduleable.Internal;

using System.Collections.Generic;
/// <summary>
/// The module container.
/// </summary>

internal class ModuleContainer : IServiceModuleContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleContainer"/> class.
    /// </summary>
    /// <param name="modules">The modules.</param>
    public ModuleContainer(IReadOnlyList<IServiceModuleDescriptor> modules) => Modules = modules;

    /// <summary>
    /// Gets the modules.
    /// </summary>
    public IReadOnlyList<IServiceModuleDescriptor> Modules { get; }
}
