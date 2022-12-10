namespace OneI.Logable;

using System;
using System.Collections.Generic;

internal static class CheckTools
{
    public static string NotNullOrEmpty(string? value)
    {
        return string.IsNullOrEmpty(value)
            ? throw new ArgumentNullException(nameof(value))
            : value!;
    }

    public static string NotNullOrWhiteSpace(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentNullException(nameof(value))
            : value!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T NotNull<T>(T? value)
    {
        return value == null
            ? throw new ArgumentNullException(nameof(value))
            : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> NotNullOrEmpty<T>(IEnumerable<T>? data)
    {
        if(data?.Any() != true)
        {
            throw new ArgumentNullException(nameof(data));
        }
        else
        {
            return data;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Dictionary<TKey, TValue> NotNullOrEmpty<TKey, TValue>(
        Dictionary<TKey, TValue>? data)
        where TKey : notnull
    {
        return data == null || !data.Any()
            ? throw new ArgumentNullException(nameof(data))
            : data;
    }
}
