namespace OneI.Moduleable;

/// <summary>
/// 模块注册器
/// </summary>
public interface IModuleInjector
{
    /// <summary>
    /// 是否保留默认的注入方法（默认：true）
    /// </summary>
    bool KeepDefault => true;

    void Scan(ModuleRegistrationContext context);
}
