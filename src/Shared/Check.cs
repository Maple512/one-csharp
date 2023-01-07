namespace OneI;

using System;
using System.Collections.Generic;
using System.IO;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string ValueBeNullMessage(
        string? filePath,
        string? memberName,
        int? lineNumber)
    {
        return $"Value cannot be null. ({Path.GetFileName(filePath)}#L{lineNumber}@{memberName})";
    }
}

#if NET7_0_OR_GREATER
[StackTraceHidden]
internal static partial class Check
{
    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullOrEmpty(
       string? value,
       [CallerFilePath] string? filePath = null,
       [CallerMemberName] string? memberName = null,
       [CallerLineNumber] int? line = null)
    {
        return string.IsNullOrEmpty(value)
            ? throw new ArgumentNullException(nameof(value), ValueBeNullMessage(filePath, memberName, line))
            : value;
    }

    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullOrWhiteSpace(
        string? value,
        [CallerFilePath] string? filePath = null,
        [CallerMemberName] string? memberName = null,
        [CallerLineNumber] int? line = null)
    {
        return string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentNullException(nameof(value), ValueBeNullMessage(filePath, memberName, line))
            : value;
    }

    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T NotNull<T>(
        T? value,
        [CallerFilePath] string? filePath = null,
        [CallerMemberName] string? memberName = null,
        [CallerLineNumber] int? line = null)
    {
        return value == null
            ? throw new ArgumentNullException(nameof(value), ValueBeNullMessage(filePath, memberName, line))
            : value;
    }

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
            throw new ArgumentNullException(nameof(data), ValueBeNullMessage(filePath, memberName, line));
        }

        return data;
    }

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
            ? throw new ArgumentNullException(nameof(data), ValueBeNullMessage(filePath, memberName, line))
            : data;
    }

    /// <summary>
    /// 当给定参数<paramref name="value"/>为<see langword="true"/>时，抛出异常
    /// <para>
    /// <example>
    /// 异常包含以下文本
    /// <code>Value("true") can not be True. at [CallerFilePath]#L[CallerLineNumber]@[CallerMemberName] (Parameter 'value')</code>
    /// </example>
    /// </para>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="expression"></param>
    /// <param name="file"></param>
    /// <param name="member"></param>
    /// <param name="line"></param>
    /// <exception cref="ArgumentException"></exception>
    public static void ThrowIfTrue(
        [DoesNotReturnIf(true)] bool value,
        [CallerArgumentExpression("value")] string? expression = null,
        [CallerFilePath] string? file = null,
        [CallerMemberName] string? member = null,
        [CallerLineNumber] int? line = null)
    {
        if(value)
        {
            throw new ArgumentException($"Value(\"{expression}\") can not be True. at {Path.GetFileName(file)}#L{line}@{member}", nameof(value));
        }
    }

    /// <summary>
    /// 当给定参数<paramref name="value"/>为<see langword="false"/>时，抛出异常
    /// <para>
    /// <example>
    /// 异常包含以下文本
    /// <code>Value("[CallerArgumentExpression("value")]") can not be False. at [CallerFilePath]#L[CallerLineNumber]@[CallerMemberName] (Parameter 'value')</code>
    /// </example>
    /// </para>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="expression"></param>
    /// <param name="file"></param>
    /// <param name="member"></param>
    /// <param name="line"></param>
    /// <exception cref="ArgumentException"></exception>
    public static void ThrowIfFalse(
        [DoesNotReturnIf(true)] bool value,
        [CallerArgumentExpression("value")] string? expression = null,
        [CallerFilePath] string? file = null,
        [CallerMemberName] string? member = null,
        [CallerLineNumber] int? line = null)
    {
        if(value == false)
        {
            throw new ArgumentException($"Value(\"{expression}\") can not be False. at {Path.GetFileName(file)}#L{line}@{member}", nameof(value));
        }
    }
}

#elif NETSTANDARD2_0_OR_GREATER
internal static partial class Check
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullOrEmpty(
       string? value,
       [CallerFilePath] string? filePath = null,
       [CallerMemberName] string? memberName = null,
       [CallerLineNumber] int? line = null)
    {
        return string.IsNullOrEmpty(value)
            ? throw new ArgumentNullException(nameof(value), ValueBeNullMessage(filePath, memberName, line))
            : value!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullOrWhiteSpace(
        string? value,
        [CallerFilePath] string? filePath = null,
        [CallerMemberName] string? memberName = null,
        [CallerLineNumber] int? line = null)
    {
        return string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentNullException(nameof(value), ValueBeNullMessage(filePath, memberName, line))
            : value!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T NotNull<T>(
        T? value,
        [CallerFilePath] string? filePath = null,
        [CallerMemberName] string? memberName = null,
        [CallerLineNumber] int? line = null)
    {
        return value == null
            ? throw new ArgumentNullException(nameof(value), ValueBeNullMessage(filePath, memberName, line))
            : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> NotNullOrEmpty<T>(
        IEnumerable<T> data,
        [CallerFilePath] string? filePath = null,
        [CallerMemberName] string? memberName = null,
        [CallerLineNumber] int? line = null)
    {
        if(data?.Any() != true)
        {
            throw new ArgumentNullException(nameof(data), ValueBeNullMessage(filePath, memberName, line));
        }
        else
        {
            return data;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Dictionary<TKey, TValue> NotNullOrEmpty<TKey, TValue>(
        Dictionary<TKey, TValue>? data,
        [CallerFilePath] string? filePath = null,
        [CallerMemberName] string? memberName = null,
        [CallerLineNumber] int? line = null)
        where TKey : notnull
    {
        return data == null || !data.Any()
            ? throw new ArgumentNullException(nameof(data), ValueBeNullMessage(filePath, memberName, line))
            : data;
    }

    public static void ThrowIfTrue(
        bool value,
        [CallerFilePath] string? file = null,
        [CallerMemberName] string? member = null,
        [CallerLineNumber] int? line = null)
    {
        if(value)
        {
            throw new ArgumentException($"Value can not be True. at {Path.GetFileName(file)}#L{line}@{member}", nameof(value));
        }
    }

    public static void ThrowIfFalse(
        bool value,
        [CallerFilePath] string? file = null,
        [CallerMemberName] string? member = null,
        [CallerLineNumber] int? line = null)
    {
        if(value == false)
        {
            throw new ArgumentException($"Value can not be False. at {Path.GetFileName(file)}#L{line}@{member}", nameof(value));
        }
    }
}
#endif
