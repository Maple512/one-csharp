namespace OneI.Applicationable;

using Microsoft.Extensions.DependencyInjection;

public interface IServiceProviderFactoryProvider<TContainerBuilder>
    where TContainerBuilder : notnull
{
    void UseServiceProviderFactory(IServiceProviderFactory<TContainerBuilder> factory);

    void UseServiceProviderFactory(Func<IApplicationBuilderContext, IServiceProviderFactory<TContainerBuilder>> factoryResolver);

    void ConfigureContainer(Action<IApplicationBuilderContext, TContainerBuilder> configure);
}
