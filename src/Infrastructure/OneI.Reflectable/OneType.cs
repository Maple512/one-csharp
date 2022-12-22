namespace OneI.Reflectable;

using System.Collections.Concurrent;
using OneI.Reflectable.Definintions;

public static class OneType
{
    private readonly static ConcurrentDictionary<int, StaticModule> _modules = new();

    public static void Add<T>()
    {
        var type = typeof(T);

        var module = new StaticModule(type.Module);

        _modules.TryAdd(module.GetHashCode(), module);


    }
}
