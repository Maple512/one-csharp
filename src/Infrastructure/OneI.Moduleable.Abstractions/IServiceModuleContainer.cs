namespace OneI.Moduleable;

/// <summary>
/// 表示服务模块容器
/// </summary>
public interface IServiceModuleContainer
{
    /// <summary>
    /// 表示所有的服务模块
    /// </summary>
    IReadOnlyList<IServiceModuleDescriptor> Modules { get; }
}
