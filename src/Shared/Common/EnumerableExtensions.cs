namespace System.Linq;

using System.Collections.Generic;
using OneI;

/// <summary>
/// The enumerable extensions.
/// </summary>
[DebuggerStepThrough]
internal static partial class EnumerableExtensions
{
    /// <summary>
    /// Joins the.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>A string.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Join<T>(this IEnumerable<T> source, string separator)
    {
        Check.NotNull(source);

        return string.Join(separator, source);
    }

    /// <summary>
    /// Wheres the if.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="condition">If true, condition.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>A list of TS.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> WhereIf<T>(
        this IEnumerable<T> source,
        bool condition,
        Func<T, bool> predicate)
    {
        Check.NotNull(source);

        return condition ? source.Where(predicate) : source;
    }

    /// <summary>
    /// Wheres the if else.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="condition">If true, condition.</param>
    /// <param name="true">The true.</param>
    /// <param name="false">The false.</param>
    /// <returns>A list of TS.</returns>
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

    /// <summary>
    /// Excludes the null and write space.
    /// </summary>
    /// <param name="sources">The sources.</param>
    /// <param name="comparer">The comparer.</param>
    /// <returns>A list of string.</returns>
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

#if NET
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetCount<T>(this IEnumerable<T> source)
    {
        if(source is ICollection collection)
        {
            return collection.Count;
        }

        if(source is ICollection<T> c)
        {
            return c.Count;
        }

        if(source.TryGetNonEnumeratedCount(out var count))
        {
            return count;
        }

        return source.Count();
    }
}

#elif NETSTANDARD2_0_OR_GREATER
internal static partial class EnumerableExtensions
{
    /// <summary>
    /// Are the null or empty.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>A bool.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
    {
        return source == null || source.Any() == false;
    }

    /// <summary>
    /// Nots the null or empty.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>A bool.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NotNullOrEmpty<T>(this IEnumerable<T>? source)
    {
        return source != null && source.Any() == true;
    }

    /// <summary>
    /// Joins the.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>A string.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Join<T>(this IEnumerable<T> source, char separator = ',')
    {
        Check.NotNull(source);

        return string.Join(char.ToString(separator), source);
    }
}
#endif
