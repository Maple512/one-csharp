namespace OneI.Moduleable;

using System.Collections.Generic;

internal class ModuleContainer
{
    public ModuleContainer(IReadOnlyList<IModuleDescriptor> modules)
    {
        Modules = modules;
    }

    public IReadOnlyList<IModuleDescriptor> Modules { get; }
}
