namespace OneI.Moduleable;

/// <summary>
/// 表示服务容器配置
/// </summary>
public interface IServiceModuleConfigureServices
{
    /// <summary>
    /// 服务容器配置
    /// </summary>
    /// <param name="context"></param>
    void ConfigureServices(in ServiceModuleConfigureServiceContext context);
}
