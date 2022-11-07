namespace OneS.Identityable;

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OneI.Eventable;
using OneI.Moduleable;
using OneS.Identityable.Users;

[ModuleDependOn(typeof(EventModule))]
public sealed class DomainModule : Module
{
    public override ValueTask ConfigureAsync(ModuleConfigureContext context)
    {
        context.ServiceProvider.TrySubscribeEvent<UserRegistrationEventData>();

        return base.ConfigureAsync(context);
    }
}
