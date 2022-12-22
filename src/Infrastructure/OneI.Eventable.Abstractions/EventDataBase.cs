namespace OneI.Eventable;

using System;
/// <summary>
/// The event data base.
/// </summary>

[Serializable]
public abstract class EventDataBase : IEventData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventDataBase"/> class.
    /// </summary>
    public EventDataBase()
    {
        Id = $"{Guid.NewGuid():n}";
        CreatedTime = DateTimeOffset.Now;
    }

    /// <summary>
    /// Gets the id.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the created time.
    /// </summary>
    public DateTimeOffset CreatedTime { get; }

    /// <summary>
    /// Gets the hash code.
    /// </summary>
    /// <returns>An int.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, CreatedTime);
    }

    /// <summary>
    /// Tos the string.
    /// </summary>
    /// <returns>A string.</returns>
    public override string ToString()
    {
        return $"[Event: {GetType().Name}, Id: {Id}, CreatedTime: {CreatedTime:G}]";
    }
}
