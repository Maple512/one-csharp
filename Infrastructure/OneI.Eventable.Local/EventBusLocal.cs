namespace OneI.Eventable;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OneI.Moduleable.DependencyInjection;

public sealed class EventBus : IEventBus, ISingletonService
{
    public EventBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        _cache = new();
    }

    private readonly IServiceProvider _serviceProvider;

    private readonly ConcurrentDictionary<Type, IEnumerable<IEventHandler>> _cache;

    public bool TrySubscribe<TEventData>() where TEventData : IEventData
    {
        var eventType = typeof(TEventData);

        return _cache.TryAdd(eventType, FindAllEventHandlers(eventType));
    }

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
