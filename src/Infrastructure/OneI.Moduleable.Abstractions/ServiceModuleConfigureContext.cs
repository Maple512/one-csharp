namespace OneI.Moduleable;

using System;

/// <summary>
/// 服务模块配置器上下文
/// </summary>
public readonly struct ServiceModuleConfigureContext
{
    public ServiceModuleConfigureContext(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    public IServiceProvider ServiceProvider { get; }

    public override int GetHashCode()
    {
        return ServiceProvider.GetHashCode();
    }

    public override string ToString()
    {
        return nameof(ServiceModuleConfigureContext);
    }
}
