namespace OneI.Eventable;

using System.Threading.Tasks;

public interface IEventBus
{
    /// <summary>
    /// 发布事件
    /// </summary>
    /// <typeparam name="TEventData"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    ValueTask PublishAsync<TEventData>(TEventData data)
        where TEventData : IEventData;

    /// <summary>
    /// 发布事件（并发执行事件处理程序）
    /// </summary>
    /// <typeparam name="TEventData"></typeparam>
    /// <param name="data"></param>
    /// <param name="parallelOptions"><see cref="Parallel"/> 中的参数</param>
    /// <returns></returns>
    ValueTask PublishAsync<TEventData>(TEventData data, ParallelOptions? parallelOptions = null)
        where TEventData : IEventData;

    /// <summary>
    /// 尝试订阅事件
    /// </summary>
    /// <typeparam name="TEventData"></typeparam>
    /// <returns><see langword="true"/>：订阅成功，<see langword="false"/>：已订阅</returns>
    bool TrySubscribe<TEventData>()
        where TEventData : IEventData;
}
