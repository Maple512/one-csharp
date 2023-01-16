namespace OneI;

using System;
using System.Collections.Generic;
using System.Linq;

[DebuggerStepThrough]
internal static partial class Check
{
    public static bool IsIn<T>(T? value, params T[] data)
    {
        if(value is null
            || data is { Length: 0 })
        {
            return false;
        }

        foreach(var item in data)
        {
            if(value.Equals(item))
            {
                return true;
            }
        }

        return false;
    }
}

#if NET
[StackTraceHidden]
internal static partial class Check
{
    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullOrEmpty(in string? value)
    {
        if(string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException(nameof(value));
        }

        return value;
    }

    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullOrWhiteSpace(in string? value)
    {
        if(string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(nameof(value));
        }

        return value!;
    }

    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T NotNull<T>(in T? value)
    {
        if(value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return value!;
    }

    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> NotNullOrEmpty<T>(IEnumerable<T> data)
    {
        if(data is null || data.Any() == false)
        {
            throw new ArgumentNullException(nameof(data));
        }

        return data;
    }
}

#elif NETSTANDARD2_0_OR_GREATER
internal static partial class Check
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullOrEmpty(in string? value )
    {
        return string.IsNullOrEmpty(value)
            ? throw new ArgumentNullException(nameof(value))
            : value!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullOrWhiteSpace(in string? value )
    {
        return string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentNullException(nameof(value))
            : value!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T NotNull<T>(in T? value)
    {
        return value == null
            ? throw new ArgumentNullException(nameof(value))
            : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> NotNullOrEmpty<T>(IEnumerable<T> data)
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
    public static Dictionary<TKey, TValue> NotNullOrEmpty<TKey, TValue>(Dictionary<TKey, TValue>? data)
        where TKey : notnull
    {
        return data == null || !data.Any()
            ? throw new ArgumentNullException(nameof(data))
            : data;
    }
}
#endif
