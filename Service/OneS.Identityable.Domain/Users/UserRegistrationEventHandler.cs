namespace OneS.Identityable.Users;

using System.Threading.Tasks;
using OneI.Eventable;
using OneI.Moduleable.DependencyInjection;

public class UserRegistrationEventHandler : EventHandlerBase<UserRegistrationEventData>, ITransientService
{
    private readonly IUserRepository _userRepository;

    public UserRegistrationEventHandler(IUserRepository userRepository) => _userRepository = userRepository;

    public override async ValueTask HandleAsync(UserRegistrationEventData data)
    {
        var user = User.CreateNew(data);

        await _userRepository.Create(user);
    }
}
