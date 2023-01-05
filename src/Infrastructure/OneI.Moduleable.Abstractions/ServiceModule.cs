namespace OneI.Moduleable;

using System.Threading.Tasks;

/// <inheritdoc cref="IServiceModule"/>
public class ServiceModule : IServiceModule
{
    public virtual void ConfigureServices(in ServiceModuleConfigureServiceContext context)
    {
    }

    public virtual void PostConfigureServices(in ServiceModuleConfigureServiceContext context)
    {
    }

    public virtual ValueTask ConfigureAsync(ServiceModuleConfigureContext context)
    {
        return ValueTask.CompletedTask;
    }
}
