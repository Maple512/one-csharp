namespace OneI.Moduleable;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
/// <summary>
/// The module descriptor.
/// </summary>

internal sealed class ModuleDescriptor : IServiceModuleDescriptor
{
    private readonly List<IServiceModuleDescriptor> _dependencies = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleDescriptor"/> class.
    /// </summary>
    /// <param name="instance">The instance.</param>
    [Obsolete]
    public ModuleDescriptor(Type instance)
    {
        StartupType = Check.NotNull(instance);

        Assembly = instance.Assembly;

        if(instance.IsAssignableTo(typeof(IServiceModule))
            && instance.TryGetParameterlessConstructor(out var constructorInfo))
        {
            Module = Expression.Lambda<Func<IServiceModule>>(Expression.New(constructorInfo)).Compile()();
        }
    }

    /// <summary>
    /// Gets the startup type.
    /// </summary>
    public Type StartupType { get; }

    /// <summary>
    /// Gets the assembly.
    /// </summary>
    public Assembly Assembly { get; }

    /// <summary>
    /// Gets the module.
    /// </summary>
    public IServiceModule? Module { get; }

    /// <summary>
    /// Gets the dependencies.
    /// </summary>
    public IReadOnlyList<IServiceModuleDescriptor> Dependencies => _dependencies;

    /// <summary>
    /// 添加模块依赖
    /// </summary>
    /// <param name="descriptor"></param>
    public void AddDependency(IServiceModuleDescriptor descriptor)
    {
        _dependencies.AddIfNotContains(descriptor);
    }

    /// <summary>
    /// Tos the string.
    /// </summary>
    /// <returns>A string.</returns>
    [Obsolete]
#pragma warning disable CS0809 // 过时成员“ModuleDescriptor.ToString()”重写未过时成员“object.ToString()”
    public override string ToString()
#pragma warning restore CS0809 // 过时成员“ModuleDescriptor.ToString()”重写未过时成员“object.ToString()”
    {
        return $"ServiceModule: {StartupType.ShortDisplayName()}";
    }
}
