namespace OneI.Moduleable;

using System.Threading.Tasks;
/// <summary>
/// The module configure.
/// </summary>

public interface IModuleConfigure
{
    /// <summary>
    /// Configures the async.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>A ValueTask.</returns>
    ValueTask ConfigureAsync(ModuleConfigureContext context);
}
