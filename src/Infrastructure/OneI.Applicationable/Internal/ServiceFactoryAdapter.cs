namespace OneI.Applicationable.Internal;

using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// The service factory adapter.
/// </summary>
internal sealed class ServiceFactoryAdapter<TContainerBuilder>
    : IServiceFactoryAdapter
    where TContainerBuilder : notnull
{
    private IServiceProviderFactory<TContainerBuilder>? _serviceProviderFactory;

    private readonly Func<ApplicationBuilderContext>? _contextResolver;
    private readonly Func<ApplicationBuilderContext, IServiceProviderFactory<TContainerBuilder>>? _factoryResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceFactoryAdapter{TContainerBuilder}"/> class.
    /// </summary>
    /// <param name="serviceProviderFactory">The service provider factory.</param>
    public ServiceFactoryAdapter(IServiceProviderFactory<TContainerBuilder> serviceProviderFactory)
        => _serviceProviderFactory = Check.NotNull(serviceProviderFactory);

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceFactoryAdapter{TContainerBuilder}"/> class.
    /// </summary>
    /// <param name="contextResolver">The context resolver.</param>
    /// <param name="factoryResolver">The factory resolver.</param>
    public ServiceFactoryAdapter(
        Func<ApplicationBuilderContext> contextResolver,
        Func<ApplicationBuilderContext, IServiceProviderFactory<TContainerBuilder>> factoryResolver)
    {
        _contextResolver = Check.NotNull(contextResolver);

        _factoryResolver = Check.NotNull(factoryResolver);
    }

    /// <summary>
    /// Creates the builder.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns>An object.</returns>
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

    /// <summary>
    /// Creates the service provider.
    /// </summary>
    /// <param name="containerBuilder">The container builder.</param>
    /// <returns>An IServiceProvider.</returns>
    public IServiceProvider CreateServiceProvider(object containerBuilder)
    {
        if(_serviceProviderFactory == null)
        {
            throw new InvalidOperationException("CreateBuilder must be called before CreateServiceProvider");
        }

        return _serviceProviderFactory.CreateServiceProvider((TContainerBuilder)containerBuilder);
    }
}
