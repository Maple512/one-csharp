namespace OneI.Applicationable.Internal;

using System;
using Microsoft.Extensions.DependencyInjection;

internal interface IServiceFactoryAdapter
{
    object CreateBuilder(IServiceCollection services);

    IServiceProvider CreateServiceProvider(object containerBuilder);
}
