namespace System.Collections.Generic;

using OneI;

#if NET7_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
[StackTraceHidden]
#endif
[DebuggerStepThrough]
public static class DictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(
        this IDictionary<TKey, TValue> directory,
        TKey key,
        TValue value) => GetOrAdd(directory, key, () => value);

    public static TValue GetOrAdd<TKey, TValue>(
        this IDictionary<TKey, TValue> directory,
        TKey key,
        Func<TValue> factory) => GetOrAdd(directory, key, k => factory());

    public static TValue GetOrAdd<TKey, TValue>(
        this IDictionary<TKey, TValue> directory,
        TKey key,
        Func<TKey, TValue> factory)
    {
        Check.NotNull(key);

        if(directory.TryGetValue(key, out var result))
        {
            return result;
        }

        return directory[key] = factory(key);
    }

    public static TValue? GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        return dictionary.TryGetValue(key, out var obj)
            ? obj
            : default;
    }
}
