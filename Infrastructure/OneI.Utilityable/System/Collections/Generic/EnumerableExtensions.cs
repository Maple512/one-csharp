namespace System.Collections.Generic;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using OneI;

[StackTraceHidden]
[DebuggerStepThrough]
public static class EnumerableExtensions
{
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this IEnumerable<T>? source)
    {
        return source == null || source.Any() == false;
    }

    public static bool NotNullOrEmpty<T>([NotNullWhen(true)] this IEnumerable<T>? source)
    {
        return source != null && source.Any() == true;
    }

    public static string JoinAsString<T>(this IEnumerable<T> source, string separator)
    {
        CheckTools.NotNull(source);

        return string.Join(separator, source);
    }

    public static string JoinAsString<T>(this IEnumerable<T> source, char separator = ',')
    {
        CheckTools.NotNull(source);

        return string.Join(separator, source);
    }

    public static IEnumerable<T> WhereIf<T>(
        this IEnumerable<T> source,
        bool condition,
        Func<T, bool> predicate)
    {
        CheckTools.NotNull(source);

        return condition ? source.Where(predicate) : source;
    }

    public static IEnumerable<T> WhereIfElse<T>(
        this IEnumerable<T> source,
        bool condition,
        Func<T, bool> @true,
        Func<T, bool> @false)
    {
        CheckTools.NotNull(source);

        return source.Where(condition ? @true : @false);
    }

    public static IEnumerable<string> FilterWhiteSpaceAndDistinct(this IEnumerable<string>? sources)
    {
        if(sources.IsNullOrEmpty())
        {
            return Array.Empty<string>();
        }

        return sources!.Where(static x => !x.IsNullOrWhiteSpace())
            .Distinct();
    }

    public static bool ContainAny<T>(this IEnumerable<T> source, params T[] items)
    {
        if(source.IsNullOrEmpty()
            || items.Length == 0)
        {
            return false;
        }

        return source.Any(x => items.Contains(x));
    }

    public static bool ContainAll<T>(this IEnumerable<T> source, params T[] items)
    {
        if(source.IsNullOrEmpty()
            || items.Length == 0)
        {
            return false;
        }

        return source.Union(items).Count() == source.Count();
    }

    public static int GetCount<T>(this IEnumerable<T> source)
    {
        if(source.TryGetNonEnumeratedCount(out var count))
        {
            return count;
        }

        return source.Count();
    }
}
