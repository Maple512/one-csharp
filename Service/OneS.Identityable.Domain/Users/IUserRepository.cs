namespace OneS.Identityable.Users;

using System.Threading.Tasks;

public interface IUserRepository
{
    ValueTask Create(User user);
}
