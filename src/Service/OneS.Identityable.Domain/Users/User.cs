namespace OneS.Identityable.Users;

using System;
using OneI.Domainable;

[Serializable]
public class User : Entity<Guid>, ICreatedTime, ILastModified
{
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的 属性“PasswordHash”必须包含非 null 值。请考虑将 属性 声明为可以为 null。
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的 属性“DisplayName”必须包含非 null 值。请考虑将 属性 声明为可以为 null。
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的 属性“EMail”必须包含非 null 值。请考虑将 属性 声明为可以为 null。
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的 属性“Name”必须包含非 null 值。请考虑将 属性 声明为可以为 null。
    private User()
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的 属性“Name”必须包含非 null 值。请考虑将 属性 声明为可以为 null。
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的 属性“EMail”必须包含非 null 值。请考虑将 属性 声明为可以为 null。
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的 属性“DisplayName”必须包含非 null 值。请考虑将 属性 声明为可以为 null。
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的 属性“PasswordHash”必须包含非 null 值。请考虑将 属性 声明为可以为 null。
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

    internal static User CreateNew(UserRegistrationEventData data)
    {
        return new(Guid.NewGuid(), data.Name, data.DisplayName, data.PasswordHash, data.EMail);
    }
}
