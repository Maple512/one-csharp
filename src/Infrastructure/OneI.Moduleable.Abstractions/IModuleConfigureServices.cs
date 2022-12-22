namespace OneI.Moduleable;
/// <summary>
/// The module configure services.
/// </summary>

public interface IModuleConfigureServices
{
    /// <summary>
    /// Configures the services.
    /// </summary>
    /// <param name="context">The context.</param>
    void ConfigureServices(in ModuleConfigureServiceContext context);
}
