namespace OneI.Eventable.Fakes;

using System.Threading.Tasks;
using OneI.Eventable;
using OneI.Moduleable.DependencyInjection;

public class SendMessageEvent : EventDataBase
{
    public SendMessageEvent(string message)
    {
        Message = message;
    }

    public string Message { get; }
}

public class SendMessageHandler1 : EventHandlerBase<SendMessageEvent>, ITransientService
{
    public override ValueTask HandleAsync(SendMessageEvent data)
    {
        EventBus_Test.Orders.Add(data.Message);

        return ValueTask.CompletedTask;
    }
}

public class SendMessageHandler2 : EventHandlerBase<SendMessageEvent>, ITransientService
{
    public override ValueTask HandleAsync(SendMessageEvent data)
    {
        EventBus_Test.Orders.Add(data.Message);

        return ValueTask.CompletedTask;
    }
}

public class SendMessageHandler3 : EventHandlerBase<SendMessageEvent>, ITransientService
{
    public override ValueTask HandleAsync(SendMessageEvent data)
    {
        EventBus_Test.Orders.Add(data.Message);

        return ValueTask.CompletedTask;
    }
}
