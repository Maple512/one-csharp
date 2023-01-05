namespace OneI.Eventable;

using Microsoft.Extensions.Logging;
using OneI.Eventable.Fakes;

public class EventBus_Test : EventTestBase
{
    private readonly IEventBus _eventBus;

    public static List<string> Orders { get; } = new();

    public EventBus_Test()
    {
        _eventBus = GetRequiredService<IEventBus>();
    }

    [Fact]
    public void Try_subscribe_event_data()
    {
        _eventBus.TrySubscribe<SendMessageEvent>().ShouldBeTrue();

        _eventBus.TrySubscribe<SendMessageEvent>().ShouldBeFalse();
    }

    [Fact]
    public void Trigger_send_message_event()
    {
        var msg = nameof(SendMessageEvent);

        Should.NotThrow(async () => await _eventBus.PublishAsync(new SendMessageEvent(msg)));

        Orders.Count.ShouldBe(GetServices<IEventHandler<SendMessageEvent>>().Count());

        Orders.Contains(msg).ShouldBeTrue();
    }
}
