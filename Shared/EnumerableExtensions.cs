namespace System.Linq;

using System.Collections.Generic;
using OneI;

[DebuggerStepThrough]
internal static partial class EnumerableExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Join<T>(this IEnumerable<T> source, string separator)
    {
        Check.NotNull(source);

        return string.Join(separator, source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> WhereIf<T>(
        this IEnumerable<T> source,
        bool condition,
        Func<T, bool> predicate)
    {
        Check.NotNull(source);

        return condition ? source.Where(predicate) : source;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> WhereIfElse<T>(
        this IEnumerable<T> source,
        bool condition,
        Func<T, bool> @true,
        Func<T, bool> @false)
    {
        Check.NotNull(source);

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

#if NET7_0_OR_GREATER
[StackTraceHidden]
internal static partial class EnumerableExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this IEnumerable<T>? source)
    {
        return source == null || source.Any() == false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NotNullOrEmpty<T>([NotNullWhen(true)] this IEnumerable<T>? source)
    {
        return source != null && source.Any() == true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Join<T>(this IEnumerable<T> source, char separator = ',')
    {
        Check.NotNull(source);

        return string.Join(separator, source);
    }
}
#elif NETSTANDARD2_0_OR_GREATER
internal static partial class EnumerableExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
    {
        return source == null || source.Any() == false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NotNullOrEmpty<T>(this IEnumerable<T>? source)
    {
        return source != null && source.Any() == true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Join<T>(this IEnumerable<T> source, char separator = ',')
    {
        Check.NotNull(source);

        return string.Join(char.ToString(separator), source);
    }
}
#endif
