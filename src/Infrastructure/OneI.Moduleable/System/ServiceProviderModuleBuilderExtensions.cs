namespace System;

using System.Threading.Tasks;
using OneI.Moduleable;
/// <summary>
/// The service provider module builder extensions.
/// </summary>

public static class ServiceProviderModuleBuilderExtensions
{
    /// <summary>
    /// Configures the async.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>A ValueTask.</returns>
    public static async ValueTask<IServiceProvider> ConfigureAsync(this IServiceProvider serviceProvider)
    {
        return await ServiceModuleBuilder.ConfigureAsync(serviceProvider);
    }
}
