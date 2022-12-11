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
        CheckTools.NotNull(key);

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

#if NETSTANDARD
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        if(dict.ContainsKey(key))
        {
            return false;
        }

        dict.Add(key, value);

        return true;
    }
#endif
}
