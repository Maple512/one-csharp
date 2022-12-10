namespace System.Collections.Generic;

internal static class DictionaryExtensions
{
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
}
