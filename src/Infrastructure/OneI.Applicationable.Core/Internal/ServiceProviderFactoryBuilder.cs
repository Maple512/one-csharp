namespace OneI.Applicationable.Internal;

using System;
using Microsoft.Extensions.DependencyInjection;

internal sealed class ServiceProviderFactoryBuilder<TContainerBuilder> : IServiceProviderFactoryBuilder
    where TContainerBuilder : notnull
{
    private readonly Action<IApplicationBuilderContext, IServiceProviderFactoryProvider<TContainerBuilder>> _provider;

    public ServiceProviderFactoryBuilder(Action<IApplicationBuilderContext, IServiceProviderFactoryProvider<TContainerBuilder>> provider)
    {
        _provider = provider;
    }

    public IServiceProvider Build(IApplicationBuilderContext context, IServiceCollection services)
    {
        var provider = new ServiceProviderFactoryProvider<TContainerBuilder>();

        _provider.Invoke(context, provider);

        var factory = provider._factory;
        if(factory is null)
        {
            factory = provider._factoryResolver!.Invoke(context);

            if(factory is null)
            {
                throw new InvalidOperationException();
            }
        }

        var containerBuilder = factory.CreateBuilder(services);

        for(var i = 0; i < provider.configureActions.Count; i++)
        {
            provider.configureActions[i].Invoke(context, containerBuilder);
        }

        return factory.CreateServiceProvider(containerBuilder);
    }
}
