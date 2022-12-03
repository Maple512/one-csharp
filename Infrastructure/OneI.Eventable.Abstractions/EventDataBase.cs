namespace OneI.Eventable;

using System;

[Serializable]
public abstract class EventDataBase : IEventData
{
    public EventDataBase()
    {
        Id = $"{Guid.NewGuid():n}";
        CreatedTime = DateTimeOffset.Now;
    }

    public string Id { get; }

    public DateTimeOffset CreatedTime { get; }

    public override int GetHashCode() => HashCode.Combine(Id, CreatedTime);

    public override string ToString() => $"[Event: {GetType().Name}, Id: {Id}, CreatedTime: {CreatedTime:G}]";
}
