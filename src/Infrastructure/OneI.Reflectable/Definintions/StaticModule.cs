namespace OneI.Reflectable.Definintions;

using System.Reflection;

internal readonly struct StaticModule
{
    public StaticModule(Module module)
    {
        FullyQualifiedName = module.FullyQualifiedName;
        MDStreamVersion = module.MDStreamVersion;
        MetadataToken = module.MetadataToken;
        ModuleHandle = module.ModuleHandle;
        ModuleVersionId = module.ModuleVersionId;
        Name = module.Name;
        ScopeName = module.ScopeName;
    }

    /// <summary>
    /// 获取表示此模块的完全限定名和路径的字符串
    /// </summary>
    public string FullyQualifiedName { get; }

    /// <summary>
    /// 获取元数据流版本
    /// </summary>
    public int MDStreamVersion { get; }

    /// <summary>
    /// 获取一个令牌，该令牌用于标识元数据中的模块
    /// </summary>
    public int MetadataToken { get; }

    /// <summary>
    /// 获取模块的图柄
    /// </summary>
    public ModuleHandle ModuleHandle { get; }

    /// <summary>
    /// 获取可用于区分模块的两个版本的全局唯一标识符 (UUID)
    /// </summary>
    public Guid ModuleVersionId { get; }

    /// <summary>
    /// 表示移除了路径的模块名
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 表示模块名的字符串
    /// </summary>
    public string ScopeName { get; }

    public override int GetHashCode()
    {
        return (Name, ScopeName, ModuleHandle, ModuleVersionId).GetHashCode();
    }

    public override string ToString()
    {
        return Name;
    }
}
