namespace OneI.Moduleable;

using System;
using System.Collections.Generic;
using System.Reflection;

public interface IModuleDescriptor
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
    IModule? Module { get; }

    /// <summary>
    /// 模块的依赖项
    /// </summary>
    IReadOnlyList<IModuleDescriptor> Dependencies { get; }

    void AddDependency(IModuleDescriptor descriptor);
}
