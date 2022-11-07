namespace OneI.Eventable;

using System.ComponentModel;
using System.Threading.Tasks;

public interface IEventHandler<in TEventData> : IEventHandler
    where TEventData : IEventData
{
    int? Order { get; }

    ValueTask HandlerAsync(TEventData data);
}

[EditorBrowsable(EditorBrowsableState.Never)]
public interface IEventHandler
{
}

public abstract class EventHandlerBase<TEventData> : IEventHandler<TEventData>
    where TEventData : IEventData
{
    public int? Order { get; }

    public abstract ValueTask HandlerAsync(TEventData data);
}
