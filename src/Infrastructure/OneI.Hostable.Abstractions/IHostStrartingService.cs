namespace OneI.Hostable;

/// <summary>
/// 表示在<see cref="IHost"/>启动时触发的服务
/// </summary>
public interface IHostStrartingService
{
    /// <summary>
    /// 当应用程序准备好时触发
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 在应用正常关闭时触发
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task StopAsync(CancellationToken cancellationToken = default);
}
