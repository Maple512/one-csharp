namespace OneI.Eventable;

using System;
using OneI.Diagnostics;

[Serializable]
public abstract class EventDataBase : IEventData
{
    public EventDataBase()
    {
        Id = $"{Guid.NewGuid():n}";

        CreatedTime = Clock.Now;
    }

    public string Id { get; }

    public DateTimeOffset CreatedTime { get; }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, CreatedTime);
    }

    public override string ToString()
    {
        return $"[Event: {GetType().Name}, Id: {Id}, CreatedTime: {CreatedTime:G}]";
    }
}
