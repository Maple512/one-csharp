namespace OneI.Moduleable;

/// <summary>
/// 服务模块
/// </summary>
/// <remarks>便于实现依赖注入</remarks>
public interface IServiceModule : IServiceModuleConfigureServices, IServiceModulePostConfigureServices, IServiceModuleConfigure
{
}
