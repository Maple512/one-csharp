namespace OneI.Eventable;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OneI.Moduleable.DependencyInjection;
/// <summary>
/// The event bus.
/// </summary>

public sealed class EventBus : IEventBus, ISingletonService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventBus"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public EventBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        _cache = new();
    }

    private readonly IServiceProvider _serviceProvider;

    private readonly ConcurrentDictionary<Type, IEnumerable<IEventHandler>> _cache;

    /// <summary>
    /// Tries the subscribe.
    /// </summary>
    /// <returns>A bool.</returns>
    public bool TrySubscribe<TEventData>() where TEventData : IEventData
    {
        var eventType = typeof(TEventData);

        return _cache.TryAdd(eventType, FindAllEventHandlers(eventType));
    }

    /// <summary>
    /// Publishes the async.
    /// </summary>
    /// <param name="eventData">The event data.</param>
    /// <returns>A ValueTask.</returns>
    public async ValueTask PublishAsync<TEventData>(TEventData eventData)
        where TEventData : IEventData
    {
        _ = Check.NotNull(eventData);

        var handlers = _cache.GetOrAdd(typeof(TEventData), FindAllEventHandlers)
            .OfType<IEventHandler<TEventData>>().OrderBy(x => x.Order);

        foreach(var handler in handlers)
        {
            await handler!.HandleAsync(eventData);

            if(handler is IDisposable disposable)
            {
                disposable.Dispose();
            }

            if(handler is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
        }
    }

    /// <summary>
    /// Publishes the async.
    /// </summary>
    /// <param name="eventData">The event data.</param>
    /// <param name="parallelOptions">The parallel options.</param>
    /// <returns>A ValueTask.</returns>
    public async ValueTask PublishAsync<TEventData>(TEventData eventData, ParallelOptions? parallelOptions = null)
        where TEventData : IEventData
    {
        _ = Check.NotNull(eventData);

        var handlers = _cache.GetOrAdd(typeof(TEventData), FindAllEventHandlers)
            .OfType<IEventHandler<TEventData>>().OrderBy(x => x.Order);

        await Parallel.ForEachAsync(handlers, parallelOptions ?? new(), async (handler, token) =>
        {
            await handler.HandleAsync(eventData);

            if(handler is IDisposable disposable)
            {
                disposable.Dispose();
            }

            if(handler is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
        });
    }

    /// <summary>
    /// Finds the all event handlers.
    /// </summary>
    /// <param name="eventType">The event type.</param>
    /// <returns>A list of IEventHandlers.</returns>
    private IEnumerable<IEventHandler> FindAllEventHandlers(Type eventType)
    {
        var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);

        var handlers = _serviceProvider.GetServices<IEventHandler>()
            .Where(x => x.GetType().IsAssignableTo(handlerType));

        if(handlers.IsNullOrEmpty())
        {
            throw new InvalidOperationException($"Could not found event handler: {handlerType.AssemblyQualifiedName}");
        }

        return handlers!;
    }
}
