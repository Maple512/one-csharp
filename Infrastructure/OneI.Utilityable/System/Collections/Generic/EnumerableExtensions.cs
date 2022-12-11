namespace System.Collections.Generic;

using System.Linq;
using OneI;

#if NET7_0_OR_GREATER
[StackTraceHidden]
#endif
[DebuggerStepThrough]
public static class EnumerableExtensions
{
#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this IEnumerable<T>? source)
        => source == null || source.Any() == false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NotNullOrEmpty<T>([NotNullWhen(true)] this IEnumerable<T>? source)
        => source != null && source.Any() == true;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string JoinAsString<T>(this IEnumerable<T> source, string separator)
    {
        CheckTools.NotNull(source);

        return string.Join(separator, source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string JoinAsString<T>(this IEnumerable<T> source, char separator = ',')
    {
        CheckTools.NotNull(source);

        return string.Join(separator, source);
    }
#elif NETSTANDARD
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
        => source == null || source.Any() == false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NotNullOrEmpty<T>(this IEnumerable<T>? source)
        => source != null && source.Any() == true;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string JoinAsString<T>(this IEnumerable<T> source, string separator)
    {
        CheckTools.NotNull(source);

        return string.Join(separator, source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string JoinAsString<T>(this IEnumerable<T> source, char separator = ',')
    {
        CheckTools.NotNull(source);

        return string.Join(char.ToString(separator), source);
    }
#endif


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> WhereIf<T>(
        this IEnumerable<T> source,
        bool condition,
        Func<T, bool> predicate)
    {
        CheckTools.NotNull(source);

        return condition ? source.Where(predicate) : source;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> WhereIfElse<T>(
        this IEnumerable<T> source,
        bool condition,
        Func<T, bool> @true,
        Func<T, bool> @false)
    {
        CheckTools.NotNull(source);

        return source.Where(condition ? @true : @false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<string> ExcludeNullAndWriteSpace(this IEnumerable<string> sources, IEqualityComparer<string>? comparer = null)
    {
        if(sources.IsNullOrEmpty())
        {
            return Array.Empty<string>();
        }

        var filtered = sources.Where(static x => x.NotNullOrWhiteSpace());

        return comparer is null
            ? filtered
            : filtered.Distinct(comparer);
    }
}
