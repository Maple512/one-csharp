namespace OneI.Moduleable;
/// <summary>
/// The module post configure services.
/// </summary>

public interface IModulePostConfigureServices
{
    /// <summary>
    /// Posts the configure services.
    /// </summary>
    /// <param name="context">The context.</param>
    void PostConfigureServices(in ModuleConfigureServiceContext context);
}
