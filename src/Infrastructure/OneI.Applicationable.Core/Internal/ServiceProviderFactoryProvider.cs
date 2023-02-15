namespace OneI.Applicationable.Internal;

using System;
using Microsoft.Extensions.DependencyInjection;

internal class ServiceProviderFactoryProvider<TContainerBuilder> : IServiceProviderFactoryProvider<TContainerBuilder>
        where TContainerBuilder : notnull
{
    internal IServiceProviderFactory<TContainerBuilder>? _factory;
    internal List<Action<IApplicationBuilderContext, TContainerBuilder>> configureActions = new();
    internal Func<IApplicationBuilderContext, IServiceProviderFactory<TContainerBuilder>>? _factoryResolver;

    public void ConfigureContainer(Action<IApplicationBuilderContext, TContainerBuilder> configure)
    {
        configureActions.Add(configure);
    }

    public void UseServiceProviderFactory(IServiceProviderFactory<TContainerBuilder> factory)
    {
        _factory = factory;
    }

    public void UseServiceProviderFactory(Func<IApplicationBuilderContext, IServiceProviderFactory<TContainerBuilder>> factoryResolver)
    {
        _factoryResolver = factoryResolver;
    }
}
