namespace OneI.Moduleable;

public interface IModulePostConfigureServices
{
    void PostConfigureServices(in ModuleConfigureServiceContext context);
}
