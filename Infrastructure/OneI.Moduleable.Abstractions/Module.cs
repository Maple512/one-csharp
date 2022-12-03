namespace OneI.Moduleable;

using System.Threading.Tasks;

/// <summary>
/// 模块
/// </summary>
/// <remarks>便于实现依赖注入</remarks>
public class Module : IModule
{
    public virtual void ConfigureServices(in ModuleConfigureServiceContext context)
    {
    }

    public virtual void PostConfigureServices(in ModuleConfigureServiceContext context)
    {
    }

    public virtual ValueTask ConfigureAsync(ModuleConfigureContext context) => ValueTask.CompletedTask;
}
