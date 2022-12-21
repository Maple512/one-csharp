namespace Microsoft.Extensions.DependencyInjection;

using System;
using System.Linq;
using System.Reflection;
using OneI;

public static class ServiceCollectionBuilderExtensions
{
    public static IServiceProvider BuildServiceProviderFromFactory(this IServiceCollection services)
    {
        _ = Check.NotNull(services);

        foreach(var service in services)
        {
            var factoryInterface = service.ImplementationInstance?.GetType()
                .GetInterfaces()
                .FirstOrDefault(
                i => i.GetTypeInfo().IsGenericType
                && i.GetGenericTypeDefinition() == typeof(IServiceProviderFactory<>));

            if(factoryInterface == null)
            {
                continue;
            }

            var containerBuilderType = factoryInterface.GenericTypeArguments[0];

            return (IServiceProvider)typeof(ServiceCollectionBuilderExtensions)
                .GetMethods()
                .Single(m => m.Name == nameof(BuildServiceProviderWithContaner))
                .MakeGenericMethod(containerBuilderType)
                .Invoke(null, new object[]
                {
                    services, null!,
                })!;
        }

        return services.BuildServiceProvider();
    }

    public static IServiceProvider BuildServiceProviderWithContaner<TContainerBuilder>(
        this IServiceCollection services,
        Action<TContainerBuilder>? builderAction = null)
        where TContainerBuilder : notnull
    {
        _ = Check.NotNull(services);

        var serviceProviderFactory = services.GetSingleInstance<IServiceProviderFactory<TContainerBuilder>>();

        var builder = serviceProviderFactory.CreateBuilder(services);

        builderAction?.Invoke(builder);

        return serviceProviderFactory.CreateServiceProvider(builder);
    }
}
