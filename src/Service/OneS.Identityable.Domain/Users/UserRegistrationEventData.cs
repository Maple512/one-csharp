namespace OneS.Identityable.Users;

using OneI.Eventable;

public class UserRegistrationEventData : EventDataBase
{
    public string Name { get; }

    public string DisplayName { get; }

    public string PasswordHash { get; }

    public string EMail { get; }
}
