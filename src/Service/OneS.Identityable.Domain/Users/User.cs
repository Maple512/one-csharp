namespace OneS.Identityable.Users;

using System;
using OneI.Domainable;

[Serializable]
public class User : Entity<Guid>, ICreatedTime, ILastModified
{
#pragma warning disable CS8618 // ���˳����캯��ʱ������Ϊ null �� ���ԡ�PasswordHash����������� null ֵ���뿼�ǽ� ���� ����Ϊ����Ϊ null��
#pragma warning disable CS8618 // ���˳����캯��ʱ������Ϊ null �� ���ԡ�DisplayName����������� null ֵ���뿼�ǽ� ���� ����Ϊ����Ϊ null��
#pragma warning disable CS8618 // ���˳����캯��ʱ������Ϊ null �� ���ԡ�EMail����������� null ֵ���뿼�ǽ� ���� ����Ϊ����Ϊ null��
#pragma warning disable CS8618 // ���˳����캯��ʱ������Ϊ null �� ���ԡ�Name����������� null ֵ���뿼�ǽ� ���� ����Ϊ����Ϊ null��
    private User()
#pragma warning restore CS8618 // ���˳����캯��ʱ������Ϊ null �� ���ԡ�Name����������� null ֵ���뿼�ǽ� ���� ����Ϊ����Ϊ null��
#pragma warning restore CS8618 // ���˳����캯��ʱ������Ϊ null �� ���ԡ�EMail����������� null ֵ���뿼�ǽ� ���� ����Ϊ����Ϊ null��
#pragma warning restore CS8618 // ���˳����캯��ʱ������Ϊ null �� ���ԡ�DisplayName����������� null ֵ���뿼�ǽ� ���� ����Ϊ����Ϊ null��
#pragma warning restore CS8618 // ���˳����캯��ʱ������Ϊ null �� ���ԡ�PasswordHash����������� null ֵ���뿼�ǽ� ���� ����Ϊ����Ϊ null��
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
