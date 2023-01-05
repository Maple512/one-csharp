namespace OneI.Moduleable;

public interface IServiceModulePostConfigureServices
{
    /// <summary>
    /// Posts the configure services.
    /// </summary>
    /// <param name="context">The context.</param>
    void PostConfigureServices(in ServiceModuleConfigureServiceContext context);
}
