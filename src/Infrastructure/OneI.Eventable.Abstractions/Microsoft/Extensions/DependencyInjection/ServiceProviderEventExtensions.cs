namespace Microsoft.Extensions.DependencyInjection;

using System;
using OneI.Eventable;

/// <summary>
/// The service provider event extensions.
/// </summary>
public static class ServiceProviderEventExtensions
{
    /// <summary>
    /// 尝试订阅事件
    /// </summary>
    /// <typeparam name="TEventData"></typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static bool TrySubscribeEvent<TEventData>(this IServiceProvider serviceProvider)
        where TEventData : IEventData
    {
        var eventBus = serviceProvider.GetRequiredService<IEventBus>();

        return eventBus.TrySubscribe<TEventData>();
    }
}
