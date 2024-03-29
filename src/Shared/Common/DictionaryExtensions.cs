namespace System.Collections.Generic;

[StackTraceHidden]
[DebuggerStepThrough]
internal static class DictionaryExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TValue GetOrAdd<TKey, TValue>(
        this IDictionary<TKey, TValue> directory,
        TKey key,
        TValue value)
    {
        if(directory.TryGetValue(key, out var result))
        {
            return result;
        }

        return directory[key] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TValue GetOrAdd<TKey, TValue>(
        this IDictionary<TKey, TValue> directory,
        TKey key,
        Func<TValue> factory)
    {
        return GetOrAdd(directory, key, _ => factory());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TValue GetOrAdd<TKey, TValue>(
        this IDictionary<TKey, TValue> directory,
        TKey key,
        Func<TKey, TValue> factory)
    {
        if(directory.TryGetValue(key, out var result))
        {
            return result;
        }

        return directory[key] = factory(key);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TValue? GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        if(dictionary.TryGetValue(key, out var obj))
        {
            return obj;
        }

        return default;
    }
}
