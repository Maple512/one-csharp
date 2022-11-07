namespace OneI.Moduleable;

public interface IModuleConfigureServices
{
    void ConfigureServices(in ModuleConfigureServiceContext context);
}
