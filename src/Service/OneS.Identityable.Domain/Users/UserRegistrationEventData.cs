namespace OneS.Identityable.Users;

using OneI.Eventable;

public class UserRegistrationEventData : EventDataBase
{
#pragma warning disable CS8618 // ���˳����캯��ʱ������Ϊ null �� ���ԡ�Name����������� null ֵ���뿼�ǽ� ���� ����Ϊ����Ϊ null��
    public string Name { get; }
#pragma warning restore CS8618 // ���˳����캯��ʱ������Ϊ null �� ���ԡ�Name����������� null ֵ���뿼�ǽ� ���� ����Ϊ����Ϊ null��

#pragma warning disable CS8618 // ���˳����캯��ʱ������Ϊ null �� ���ԡ�DisplayName����������� null ֵ���뿼�ǽ� ���� ����Ϊ����Ϊ null��
    public string DisplayName { get; }
#pragma warning restore CS8618 // ���˳����캯��ʱ������Ϊ null �� ���ԡ�DisplayName����������� null ֵ���뿼�ǽ� ���� ����Ϊ����Ϊ null��

#pragma warning disable CS8618 // ���˳����캯��ʱ������Ϊ null �� ���ԡ�PasswordHash����������� null ֵ���뿼�ǽ� ���� ����Ϊ����Ϊ null��
    public string PasswordHash { get; }
#pragma warning restore CS8618 // ���˳����캯��ʱ������Ϊ null �� ���ԡ�PasswordHash����������� null ֵ���뿼�ǽ� ���� ����Ϊ����Ϊ null��

#pragma warning disable CS8618 // ���˳����캯��ʱ������Ϊ null �� ���ԡ�EMail����������� null ֵ���뿼�ǽ� ���� ����Ϊ����Ϊ null��
    public string EMail { get; }
#pragma warning restore CS8618 // ���˳����캯��ʱ������Ϊ null �� ���ԡ�EMail����������� null ֵ���뿼�ǽ� ���� ����Ϊ����Ϊ null��
}
