namespace OneS.Identityable.Users;

using OneI.Eventable;

public class UserRegistrationEventData : EventDataBase
{
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的 属性“Name”必须包含非 null 值。请考虑将 属性 声明为可以为 null。
    public string Name { get; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的 属性“Name”必须包含非 null 值。请考虑将 属性 声明为可以为 null。

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的 属性“DisplayName”必须包含非 null 值。请考虑将 属性 声明为可以为 null。
    public string DisplayName { get; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的 属性“DisplayName”必须包含非 null 值。请考虑将 属性 声明为可以为 null。

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的 属性“PasswordHash”必须包含非 null 值。请考虑将 属性 声明为可以为 null。
    public string PasswordHash { get; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的 属性“PasswordHash”必须包含非 null 值。请考虑将 属性 声明为可以为 null。

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的 属性“EMail”必须包含非 null 值。请考虑将 属性 声明为可以为 null。
    public string EMail { get; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的 属性“EMail”必须包含非 null 值。请考虑将 属性 声明为可以为 null。
}
