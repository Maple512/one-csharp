namespace OneI.Moduleable;

/// <summary>
/// 模块注入器
/// </summary>
public interface IServiceModuleInjector
{
    /// <summary>
    /// 是否保留默认的注入方法（默认：true）
    /// </summary>
    bool KeepDefault => true;

    /// <summary>
    /// Injects the.
    /// </summary>
    /// <param name="context">The context.</param>
    void Inject(ServiceModuleRegistrationContext context);
}
