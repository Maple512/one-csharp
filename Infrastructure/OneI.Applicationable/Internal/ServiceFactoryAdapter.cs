namespace OneI.Applicationable.Internal;

using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

internal sealed class ServiceFactoryAdapter<TContainerBuilder>
    : IServiceFactoryAdapter
    where TContainerBuilder : notnull
{
    private IServiceProviderFactory<TContainerBuilder>? _serviceProviderFactory;

    private readonly Func<ApplicationBuilderContext>? _contextResolver;
    private readonly Func<ApplicationBuilderContext, IServiceProviderFactory<TContainerBuilder>>? _factoryResolver;

    public ServiceFactoryAdapter(IServiceProviderFactory<TContainerBuilder> serviceProviderFactory) => _serviceProviderFactory = CheckTools.NotNull(serviceProviderFactory);

    public ServiceFactoryAdapter(
        Func<ApplicationBuilderContext> contextResolver,
        Func<ApplicationBuilderContext, IServiceProviderFactory<TContainerBuilder>> factoryResolver)
    {
        _contextResolver = CheckTools.NotNull(contextResolver);

        _factoryResolver = CheckTools.NotNull(factoryResolver);
    }

    public object CreateBuilder(IServiceCollection services)
    {
        if(_serviceProviderFactory == null)
        {
            Debug.Assert(_factoryResolver != null
                && _contextResolver != null);

            _serviceProviderFactory = _factoryResolver(_contextResolver());

            if(_serviceProviderFactory == null)
            {
                throw new InvalidOperationException("The resolver returned a null IServiceProviderFactory");
            }
        }

        return _serviceProviderFactory.CreateBuilder(services);
    }

    public IServiceProvider CreateServiceProvider(object containerBuilder)
    {
        if(_serviceProviderFactory == null)
        {
            throw new InvalidOperationException("CreateBuilder must be called before CreateServiceProvider");
        }

        return _serviceProviderFactory.CreateServiceProvider((TContainerBuilder)containerBuilder);
    }
}
