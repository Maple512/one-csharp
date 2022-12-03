namespace OneS.Identityable.Users;

using System;
using OneI.Domainable;

[Serializable]
public class User : Entity<Guid>, ICreatedTime, ILastModified
{
    private User()
    {
    }

    private User(Guid id, string name, string displayName, string passwordHash, string email) : base(id)
    {
        Name = name;
        DisplayName = displayName;
        PasswordHash = passwordHash;
        EMail = email;
        CreatedTime = DateTimeOffset.Now;
        LastModifiedTime = DateTimeOffset.Now;
    }

    public string Name { get; }

    public string DisplayName { get; }

    public string PasswordHash { get; }

    public string EMail { get; }

    public DateTimeOffset CreatedTime { get; }

    public DateTimeOffset LastModifiedTime { get; }

    internal static User CreateNew(UserRegistrationEventData data) => new(Guid.NewGuid(), data.Name, data.DisplayName, data.PasswordHash, data.EMail);
}
