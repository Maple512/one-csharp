namespace System;

using System.Threading.Tasks;
using OneI.Moduleable;

public static class ServiceProviderModuleBuilderExtensions
{
    public static async ValueTask<IServiceProvider> ConfigureAsync(this IServiceProvider serviceProvider)
    {
        return await ModuleBuilder.ConfigureAsync(serviceProvider);
    }
}
