namespace OneI.Moduleable;

using System.Threading.Tasks;

/// <summary>
/// 服务模块配置器
/// </summary>
public interface IServiceModuleConfigure
{
    /// <summary>
    /// 模块配置
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    ValueTask ConfigureAsync(ServiceModuleConfigureContext context);
}
