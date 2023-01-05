namespace OneI.Moduleable.Internal;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OneI.Moduleable.DependencyInjection;

/// <summary>
/// The module injector tools.
/// </summary>
internal static class ModuleInjectorTools
{
    /// <summary>
    /// Registrations the module.
    /// </summary>
    /// <param name="context">The context.</param>
    public static void RegistrationModule(ServiceModuleRegistrationContext context)
    {
        var types = context.Assembly.GetTypes().Where(IsAllowRegistration).ToArray();

        RegisterTypes(context.Services, types);
    }

    /// <summary>
    /// Registers the types.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="types">The types.</param>
    private static void RegisterTypes(IServiceCollection services, params Type[] types)
    {
        foreach(var type in types)
        {
            RegisterType(services, type);
        }
    }

    /// <summary>
    /// Registers the type.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="type">The type.</param>
    [Obsolete]
    private static void RegisterType(IServiceCollection services, Type type)
    {
        var serviceDescribe = type.GetCustomAttribute<ServiceDescribeAttribute>();
        if(serviceDescribe?.IsDisabled == true)
        {
            return;
        }

        var lifetime = serviceDescribe?.Lifetime ?? GetServiceLifetimeFromInterfaces(type);
        if(lifetime == null)
        {
            return;
        }

        var serviceTypes = GetRegistrableServiceTypes(type, serviceDescribe);

        foreach(var serviceType in serviceTypes)
        {
            var serviceDescriptor = CreateServiceDescriptor(
                serviceType,
                type,
                serviceTypes,
                lifetime.Value);

            if(serviceDescribe?.ReplaceFirst == true)
            {
                _ = services.Replace(serviceDescriptor);
            }
            else if(serviceDescribe?.TryRegister == true)
            {
                services.TryAdd(serviceDescriptor);
            }
            else
            {
                services.Add(serviceDescriptor);
            }
        }
    }

    /// <summary>
    /// 获取服务生命周期
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static ServiceLifetime? GetServiceLifetimeFromInterfaces(Type type)
    {
        if(typeof(ITransientService).IsAssignableFrom(type))
        {
            return ServiceLifetime.Transient;
        }

        if(typeof(IScopedService).IsAssignableFrom(type))
        {
            return ServiceLifetime.Scoped;
        }

        if(typeof(ISingletonService).IsAssignableFrom(type))
        {
            return ServiceLifetime.Singleton;
        }

        return null;
    }

    private static readonly Type[] _injectables = new[] { typeof(ITransientService), typeof(IScopedService), typeof(ISingletonService), typeof(IInjectableService) };

    /// <summary>
    /// 获取可注册的服务类型
    /// </summary>
    /// <param name="type"></param>
    /// <param name="serviceRegistrable"></param>
    /// <returns></returns>
    [Obsolete]
    private static IEnumerable<Type> GetRegistrableServiceTypes(Type type, ServiceDescribeAttribute? serviceRegistrable = null)
    {
        var interfaces = type.GetInterfaces();

        // 如果只是继承Transient/Scoped/Singleton这三个其中一个接口，那么就直接注入类
        if(interfaces.Except(_injectables).IsNullOrEmpty()
            && interfaces.Length >= 2
                || interfaces.Length == 0
                    && type.IsDefined<ServiceDescribeAttribute>())
        {
            serviceRegistrable = ServiceDescribeAttribute.Onlyself;
        }

        serviceRegistrable ??= ServiceDescribeAttribute.Default;

        if(serviceRegistrable.IsDisabled)
        {
            return Enumerable.Empty<Type>();
        }

        // +1： if IncludeSelf is true
        var services = new List<Type>(interfaces.Length + 1);

        if(serviceRegistrable.IncludeOnlySelf)
        {
            services.Add(type);
        }
        else
        {
            services.AddRange(interfaces.Except(_injectables));

            if(serviceRegistrable.IncludeSelf)
            {
                services.Add(type);
            }
        }

        return services;
    }

    /// <summary>
    /// 创建服务自述器
    /// </summary>
    /// <param name="serviceType"></param>
    /// <param name="implementationType"></param>
    /// <param name="allServiceTypes"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    private static ServiceDescriptor CreateServiceDescriptor(
        Type serviceType,
        Type implementationType,
        IEnumerable<Type> allServiceTypes,
        ServiceLifetime lifetime)
    {
        if(!Check.IsIn(lifetime, ServiceLifetime.Singleton, ServiceLifetime.Scoped))
        {
            return ServiceDescriptor.Describe(
                serviceType,
                implementationType,
                lifetime
            );
        }

        var redirectedType = GetRedirectedTypeOrNull(
            serviceType,
            implementationType,
            allServiceTypes
        );

        if(redirectedType != null)
        {
            return ServiceDescriptor.Describe(
                serviceType,
                provider => provider.GetService(redirectedType)!,
                lifetime
            );
        }

        return ServiceDescriptor.Describe(
            serviceType,
            implementationType,
            lifetime
        );
    }

    /// <summary>
    /// Gets the redirected type or null.
    /// </summary>
    /// <param name="serviceType">The service type.</param>
    /// <param name="implementationType">The implementation type.</param>
    /// <param name="serviceTypes">The service types.</param>
    /// <returns>A Type? .</returns>
    private static Type? GetRedirectedTypeOrNull(
        Type serviceType,
        Type implementationType,
        IEnumerable<Type> serviceTypes)
    {
        if(serviceTypes.Count() < 2)
        {
            return null;
        }

        if(serviceType == implementationType)
        {
            return null;
        }

        if(serviceTypes.Contains(implementationType))
        {
            return implementationType;
        }

        return serviceTypes.FirstOrDefault(x => x != serviceType && serviceType.IsAssignableFrom(x));
    }

    /// <summary>
    /// 是否允许注册指定的类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static bool IsAllowRegistration(Type type)
    {
        return type.IsClass
               && !type.IsAbstract
               && !type.IsGenericType
               && (type.IsAssignableTo(typeof(IInjectableService)) || type.IsDefined(typeof(ServiceDescribeAttribute)));
    }
}
