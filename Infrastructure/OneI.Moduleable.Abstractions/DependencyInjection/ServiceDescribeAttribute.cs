namespace OneI.Moduleable.DependencyInjection;

using System;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 标注服务是否可以并如何注册到容器
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ServiceDescribeAttribute : Attribute
{
    private ServiceDescribeAttribute() { }

    public ServiceDescribeAttribute(ServiceLifetime lifetime) => Lifetime = lifetime;

    /// <summary>
    /// 默认配置
    /// </summary>
    public static readonly ServiceDescribeAttribute Default = new();

    public static readonly ServiceDescribeAttribute Onlyself = new() { IncludeOnlySelf = true };

    /// <summary>
    /// 服务生命周期
    /// </summary>
    public ServiceLifetime? Lifetime { get; init; }

    /// <summary>
    /// 尝试注册，如果已有，则不注册
    /// </summary>
    public bool TryRegister { get; init; }

    /// <summary>
    /// 禁止该服务注册到容器
    /// </summary>
    public bool IsDisabled { get; init; }

    /// <summary>
    /// 替换第一个注册到容器的同类型的服务
    /// </summary>
    public bool ReplaceFirst { get; init; }

    /// <summary>
    /// 注册时，是否除了注册服务接口（如果有），还要注册包括服务实现类型自身
    /// </summary>
    public bool IncludeSelf { get; init; }

    /// <summary>
    /// 是否只注册服务自身，不包括继承的接口（如果有）
    /// </summary>
    public bool IncludeOnlySelf { get; init; }
}
