namespace OneI.Moduleable;

using System.Collections.Generic;

public interface IModuleContainer
{
    IReadOnlyList<IModuleDescriptor> Modules { get; }
}
