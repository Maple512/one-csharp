namespace OneS.Identityable;

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OneI.Eventable;
using OneI.Moduleable;
using OneS.Identityable.Users;

[ServiceModule(typeof(EventModule))]
public sealed class DomainModule : ServiceModule
{
    public override ValueTask ConfigureAsync(ServiceModuleConfigureContext context)
    {
        context.ServiceProvider.TrySubscribeEvent<UserRegistrationEventData>();

        return base.ConfigureAsync(context);
    }
}
