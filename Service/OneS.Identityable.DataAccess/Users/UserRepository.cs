namespace OneS.Identityable.Users;

using System;
using System.Threading.Tasks;
using OneI.Moduleable.DependencyInjection;

public class UserRepository : IUserRepository, ITransientService
{
    public ValueTask Create(User user) => throw new NotImplementedException();
}
