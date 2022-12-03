namespace OneI.Moduleable.Internal;

using System.Collections.Generic;

internal class ModuleContainer : IModuleContainer
{
    public ModuleContainer(IReadOnlyList<IModuleDescriptor> modules) => Modules = modules;

    public IReadOnlyList<IModuleDescriptor> Modules { get; }
}
