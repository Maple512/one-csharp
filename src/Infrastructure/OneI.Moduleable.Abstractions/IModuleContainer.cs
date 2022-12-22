namespace OneI.Moduleable;

using System.Collections.Generic;
/// <summary>
/// The module container.
/// </summary>

public interface IModuleContainer
{
    /// <summary>
    /// Gets the modules.
    /// </summary>
    IReadOnlyList<IModuleDescriptor> Modules { get; }
}
