namespace System.Linq;

using OneI;

[StackTraceHidden]
[DebuggerStepThrough]
internal static partial class EnumerableExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Join<T>(this IEnumerable<T> source, string separator)
    {
        ThrowHelper.ThrowIfNull(source);

        return string.Join(separator, source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> WhereIf<T>(
        this IEnumerable<T> source,
        bool condition,
        Func<T, bool> predicate)
    {
        ThrowHelper.ThrowIfNull(source);

        return condition ? source.Where(predicate) : source;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> WhereIfElse<T>(
        this IEnumerable<T> source,
        bool condition,
        Func<T, bool> @true,
        Func<T, bool> @false)
    {
        ThrowHelper.ThrowIfNull(source);

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this IEnumerable<T>? source)
    {
        return source == null || source.Any() == false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NotNullOrEmpty<T>([NotNullWhen(true)] this IEnumerable<T>? source)
    {
        return source != null && source.Any();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Join<T>(this IEnumerable<T> source, char separator = ',')
    {
        ThrowHelper.ThrowIfNull(source);

#if NET
        return string.Join(separator, source);
#else
        return string.Join(separator.ToString(), source);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetCount<T>(this IEnumerable<T> source)
    {
        switch(source)
        {
            case ICollection collection:
                return collection.Count;
            case ICollection<T> c:
                return c.Count;
        }

        if(source.TryGetNonEnumeratedCount(out var count))
        {
            return count;
        }

        return source.Count();
    }
}
