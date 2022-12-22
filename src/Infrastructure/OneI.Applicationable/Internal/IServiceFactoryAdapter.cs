namespace OneI.Applicationable.Internal;

using System;
using Microsoft.Extensions.DependencyInjection;
/// <summary>
/// The service factory adapter.
/// </summary>

internal interface IServiceFactoryAdapter
{
    /// <summary>
    /// Creates the builder.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns>An object.</returns>
    object CreateBuilder(IServiceCollection services);

    /// <summary>
    /// Creates the service provider.
    /// </summary>
    /// <param name="containerBuilder">The container builder.</param>
    /// <returns>An IServiceProvider.</returns>
    IServiceProvider CreateServiceProvider(object containerBuilder);
}
