namespace OneI.Eventable;

using System.ComponentModel;
using System.Threading.Tasks;

/// <summary>
/// The event handler.
/// </summary>

public interface IEventHandler<in TEventData> : IEventHandler
    where TEventData : IEventData
{
    /// <summary>
    /// Gets the order.
    /// </summary>
    int? Order { get; }

    /// <summary>
    /// Handles the async.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>A ValueTask.</returns>
    ValueTask HandleAsync(TEventData data);
}
/// <summary>
/// The event handler.
/// </summary>

[EditorBrowsable(EditorBrowsableState.Never)]
public interface IEventHandler
{
}
/// <summary>
/// The event handler base.
/// </summary>

public abstract class EventHandlerBase<TEventData> : IEventHandler<TEventData>
    where TEventData : IEventData
{
    /// <summary>
    /// Gets the order.
    /// </summary>
    public int? Order { get; }

    /// <summary>
    /// Handles the async.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>A ValueTask.</returns>
    public abstract ValueTask HandleAsync(TEventData data);
}
