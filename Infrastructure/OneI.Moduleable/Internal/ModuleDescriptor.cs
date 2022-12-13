namespace OneI.Moduleable;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

internal sealed class ModuleDescriptor : IModuleDescriptor
{
    private readonly List<IModuleDescriptor> _dependencies = new();

    public ModuleDescriptor(Type instance)
    {
        StartupType = Check.NotNull(instance);

        Assembly = instance.Assembly;

        if(instance.IsAssignableTo<IModule>()
            && instance.TryGetParameterlessConstructor(out var constructorInfo))
        {
            Module = Expression.Lambda<Func<IModule>>(Expression.New(constructorInfo)).Compile()();
        }
    }

    public Type StartupType { get; }

    public Assembly Assembly { get; }

    public IModule? Module { get; }

    public IReadOnlyList<IModuleDescriptor> Dependencies => _dependencies;

    /// <summary>
    /// 添加模块依赖
    /// </summary>
    /// <param name="descriptor"></param>
    public void AddDependency(IModuleDescriptor descriptor) => _dependencies.AddIfNotContains(descriptor);

    public override string ToString() => $"Module: {StartupType.ShortDisplayName()}";
}
