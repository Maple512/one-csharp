namespace System.Collections.Generic;

using OneI;

/// <summary>
/// The dictionary extensions.
/// </summary>
#if NET7_0_OR_GREATER
[StackTraceHidden]
#endif
[DebuggerStepThrough]
internal static class DictionaryExtensions
{
    /// <summary>
    /// Gets the or add.
    /// </summary>
    /// <param name="directory">The directory.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>A TValue.</returns>
    public static TValue GetOrAdd<TKey, TValue>(
        this IDictionary<TKey, TValue> directory,
        TKey key,
        TValue value)
    {
        return GetOrAdd(directory, key, () => value);
    }

    /// <summary>
    /// Gets the or add.
    /// </summary>
    /// <param name="directory">The directory.</param>
    /// <param name="key">The key.</param>
    /// <param name="factory">The factory.</param>
    /// <returns>A TValue.</returns>
    public static TValue GetOrAdd<TKey, TValue>(
        this IDictionary<TKey, TValue> directory,
        TKey key,
        Func<TValue> factory)
    {
        return GetOrAdd(directory, key, k => factory());
    }

    /// <summary>
    /// Gets the or add.
    /// </summary>
    /// <param name="directory">The directory.</param>
    /// <param name="key">The key.</param>
    /// <param name="factory">The factory.</param>
    /// <returns>A TValue.</returns>
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

    /// <summary>
    /// Gets the or default.
    /// </summary>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <returns>A TValue? .</returns>
    public static TValue? GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        return dictionary.TryGetValue(key, out var obj)
            ? obj
            : default;
    }
}
