namespace OneI.Moduleable;

using System.Threading.Tasks;

/// <summary>
/// 模块
/// </summary>
/// <remarks>便于实现依赖注入</remarks>
public class Module : IModule
{
    /// <summary>
    /// Configures the services.
    /// </summary>
    /// <param name="context">The context.</param>
    public virtual void ConfigureServices(in ModuleConfigureServiceContext context)
    {
    }

    /// <summary>
    /// Posts the configure services.
    /// </summary>
    /// <param name="context">The context.</param>
    public virtual void PostConfigureServices(in ModuleConfigureServiceContext context)
    {
    }

    /// <summary>
    /// Configures the async.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>A ValueTask.</returns>
    public virtual ValueTask ConfigureAsync(ModuleConfigureContext context)
    {
        return ValueTask.CompletedTask;
    }
}
