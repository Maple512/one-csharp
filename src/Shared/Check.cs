namespace OneI;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The check.
/// </summary>
[DebuggerStepThrough]
internal static partial class Check
{
    /// <summary>
    /// Are the in.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="data">The data.</param>
    /// <returns>A bool.</returns>
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

    /// <summary>
    /// Errors the message.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="memberName">The member name.</param>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>A string.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string ErrorMessage(
        string? filePath,
        string? memberName,
        int? lineNumber)
    {
        return $"Value be not null. ({filePath}#L{lineNumber}@{memberName})";
    }
}

#if NET7_0_OR_GREATER

/// <summary>
/// The check.
/// </summary>
[StackTraceHidden]
internal static partial class Check
{
    /// <summary>
    /// Nots the null or empty.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="memberName">The member name.</param>
    /// <param name="line">The line.</param>
    /// <returns>A string.</returns>
    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullOrEmpty(
       string? value,
       [CallerFilePath] string? filePath = null,
       [CallerMemberName] string? memberName = null,
       [CallerLineNumber] int? line = null)
    {
        return string.IsNullOrEmpty(value)
            ? throw new ArgumentNullException(nameof(value), ErrorMessage(filePath, memberName, line))
            : value;
    }

    /// <summary>
    /// Nots the null or white space.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="memberName">The member name.</param>
    /// <param name="line">The line.</param>
    /// <returns>A string.</returns>
    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullOrWhiteSpace(
        string? value,
        [CallerFilePath] string? filePath = null,
        [CallerMemberName] string? memberName = null,
        [CallerLineNumber] int? line = null)
    {
        return string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentNullException(nameof(value), ErrorMessage(filePath, memberName, line))
            : value;
    }

    /// <summary>
    /// Nots the null.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="memberName">The member name.</param>
    /// <param name="line">The line.</param>
    /// <returns>A T.</returns>
    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T NotNull<T>(
        T? value,
        [CallerFilePath] string? filePath = null,
        [CallerMemberName] string? memberName = null,
        [CallerLineNumber] int? line = null)
    {
        return value == null
            ? throw new ArgumentNullException(nameof(value), ErrorMessage(filePath, memberName, line))
            : value;
    }

    /// <summary>
    /// Nots the null or empty.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="memberName">The member name.</param>
    /// <param name="line">The line.</param>
    /// <returns>A list of TS.</returns>
    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> NotNullOrEmpty<T>(
        IEnumerable<T> data,
        [CallerFilePath] string? filePath = null,
        [CallerMemberName] string? memberName = null,
        [CallerLineNumber] int? line = null)
    {
        if(data?.Any() != true)
        {
            throw new ArgumentNullException(nameof(data), ErrorMessage(filePath, memberName, line));
        }
        else
        {
            return data;
        }
    }

    /// <summary>
    /// Nots the null or empty.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="memberName">The member name.</param>
    /// <param name="line">The line.</param>
    /// <returns>A Dictionary.</returns>
    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Dictionary<TKey, TValue> NotNullOrEmpty<TKey, TValue>(
        Dictionary<TKey, TValue>? data,
        [CallerFilePath] string? filePath = null,
        [CallerMemberName] string? memberName = null,
        [CallerLineNumber] int? line = null)
        where TKey : notnull
    {
        return data == null || !data.Any()
            ? throw new ArgumentNullException(nameof(data), ErrorMessage(filePath, memberName, line))
            : data;
    }
}

#elif NETSTANDARD2_0_OR_GREATER
/// <summary>
/// The check.
/// </summary>

internal static partial class Check
{
    /// <summary>
    /// Nots the null or empty.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="memberName">The member name.</param>
    /// <param name="line">The line.</param>
    /// <returns>A string.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullOrEmpty(
       string? value,
       [CallerFilePath] string? filePath = null,
       [CallerMemberName] string? memberName = null,
       [CallerLineNumber] int? line = null)
    {
        return string.IsNullOrEmpty(value)
            ? throw new ArgumentNullException(nameof(value), ErrorMessage(filePath, memberName, line))
            : value!;
    }

    /// <summary>
    /// Nots the null or white space.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="memberName">The member name.</param>
    /// <param name="line">The line.</param>
    /// <returns>A string.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullOrWhiteSpace(
        string? value,
        [CallerFilePath] string? filePath = null,
        [CallerMemberName] string? memberName = null,
        [CallerLineNumber] int? line = null)
    {
        return string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentNullException(nameof(value), ErrorMessage(filePath, memberName, line))
            : value!;
    }

    /// <summary>
    /// Nots the null.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="memberName">The member name.</param>
    /// <param name="line">The line.</param>
    /// <returns>A T.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T NotNull<T>(
        T? value,
        [CallerFilePath] string? filePath = null,
        [CallerMemberName] string? memberName = null,
        [CallerLineNumber] int? line = null)
    {
        return value == null
            ? throw new ArgumentNullException(nameof(value), ErrorMessage(filePath, memberName, line))
            : value;
    }

    /// <summary>
    /// Nots the null or empty.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="memberName">The member name.</param>
    /// <param name="line">The line.</param>
    /// <returns>A list of TS.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> NotNullOrEmpty<T>(
        IEnumerable<T> data,
        [CallerFilePath] string? filePath = null,
        [CallerMemberName] string? memberName = null,
        [CallerLineNumber] int? line = null)
    {
        if(data?.Any() != true)
        {
            throw new ArgumentNullException(nameof(data), ErrorMessage(filePath, memberName, line));
        }
        else
        {
            return data;
        }
    }

    /// <summary>
    /// Nots the null or empty.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="memberName">The member name.</param>
    /// <param name="line">The line.</param>
    /// <returns>A Dictionary.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Dictionary<TKey, TValue> NotNullOrEmpty<TKey, TValue>(
        Dictionary<TKey, TValue>? data,
        [CallerFilePath] string? filePath = null,
        [CallerMemberName] string? memberName = null,
        [CallerLineNumber] int? line = null)
        where TKey : notnull
    {
        return data == null || !data.Any()
            ? throw new ArgumentNullException(nameof(data), ErrorMessage(filePath, memberName, line))
            : data;
    }
}
#endif
