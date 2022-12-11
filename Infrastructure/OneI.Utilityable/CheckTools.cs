namespace OneI;

#if NET7_0_OR_GREATER
using System.ComponentModel.DataAnnotations;
[StackTraceHidden]
#endif
[DebuggerStepThrough]
public static class CheckTools
{
#if NET7_0_OR_GREATER
    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullOrEmpty(
        string? value,
        [CallerArgumentExpression("value")] string? expression = null,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int? line = null)
    {
        return string.IsNullOrEmpty(value)
            ? throw new ArgumentNullException(expression, ErrorMessage(memberName, filePath, line))
            : value;
    }

    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullOrWhiteSpace(
        string? value,
        [CallerArgumentExpression("value")] string? expression = null,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int? line = null)
    {
        return string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentNullException(expression, ErrorMessage(memberName, filePath, line))
            : value;
    }

    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T NotNull<T>(
        T? value,
        [CallerArgumentExpression("value")] string? expression = null,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int? line = null)
    {
        return value == null
            ? throw new ArgumentNullException(expression, ErrorMessage(memberName, filePath, line))
            : value;
    }

    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> NotNullOrEmpty<T>(
        IEnumerable<T>? data,
        [CallerArgumentExpression("data")] string? expression = null,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int? line = null)
    {
        if(data?.Any() != true)
        {
            throw new ArgumentNullException(expression, ErrorMessage(memberName, filePath, line));
        }
        else
        {
            return data;
        }
    }

    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Dictionary<TKey, TValue> NotNullOrEmpty<TKey, TValue>(
        Dictionary<TKey, TValue>? data,
        [CallerArgumentExpression("data")] string? expression = null,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int? line = null)
        where TKey : notnull
    {
        return data == null || !data.Any()
            ? throw new ArgumentNullException(expression, ErrorMessage(memberName, filePath, line))
            : data;
    }

    public static bool IsIn<T>([NotNullWhen(true)] T? value, params T[] data)
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

#elif NETSTANDARD
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullOrEmpty(
        string? value,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int? line = null)
    {
        return string.IsNullOrEmpty(value)
            ? throw new ArgumentNullException(nameof(value), ErrorMessage(memberName, filePath, line))
            : value!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NotNullOrWhiteSpace(
        string? value,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int? line = null)
    {
        return string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentNullException(nameof(value), ErrorMessage(memberName, filePath, line))
            : value!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T NotNull<T>(
        T? value,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int? line = null)
    {
        return value == null
            ? throw new ArgumentNullException(nameof(value), ErrorMessage(memberName, filePath, line))
            : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> NotNullOrEmpty<T>(
        IEnumerable<T>? data,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int? line = null)
    {
        if(data?.Any() != true)
        {
            throw new ArgumentNullException(nameof(data), ErrorMessage(memberName, filePath, line));
        }
        else
        {
            return data;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Dictionary<TKey, TValue> NotNullOrEmpty<TKey, TValue>(
        Dictionary<TKey, TValue>? data,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filePath = null,
        [CallerLineNumber] int? line = null)
        where TKey : notnull
    {
        return data == null || !data.Any()
            ? throw new ArgumentNullException(nameof(data), ErrorMessage(memberName, filePath, line))
            : data;
    }

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

#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string ErrorMessage(
        string? memberName,
        string? filePath,
        int? lineNumber)
    {
        return $"Value be not null. (\"{filePath}\" L{lineNumber} \"{memberName}\")";
    }
}
