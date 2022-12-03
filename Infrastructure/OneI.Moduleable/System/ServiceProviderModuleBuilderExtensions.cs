namespace System;

using System.Threading.Tasks;
using OneI.Moduleable;

public static class ServiceProviderModuleBuilderExtensions
{
    public static async ValueTask<IServiceProvider> ConfigureAsync(this IServiceProvider serviceProvider) => await ModuleBuilder.ConfigureAsync(serviceProvider);
}
