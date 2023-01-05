namespace OneI.Moduleable;

using System.Reflection;

/// <summary>
/// 表示服务模块描述符
/// </summary>
public interface IServiceModuleDescriptor
{
    /// <summary>
    /// 模块类型
    /// </summary>
    Type StartupType { get; }

    /// <summary>
    /// 模块所在的程序集
    /// </summary>
    Assembly Assembly { get; }

    /// <summary>
    /// 模块的实例
    /// </summary>
    IServiceModule? Module { get; }

    /// <summary>
    /// 模块的依赖项
    /// </summary>
    IReadOnlyList<IServiceModuleDescriptor> Dependencies { get; }

    void AddDependency(IServiceModuleDescriptor descriptor);
}
