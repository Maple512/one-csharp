namespace System.Collections.Generic;

using OneI;

[StackTraceHidden]
[DebuggerStepThrough]
public static class EnumerableExtensions
{
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this IEnumerable<T>? source)
        => source == null || source.Any() == false;

    public static bool NotNullOrEmpty<T>([NotNullWhen(true)] this IEnumerable<T>? source)
        => source != null && source.Any() == true;

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

    public static IEnumerable<string> ExcludeNullAndWriteSpace(this IEnumerable<string> sources, IEqualityComparer<string>? comparer = null)
    {
        if(sources.IsNullOrEmpty())
        {
            return Array.Empty<string>();
        }

        var filtered = sources.Where(static x => x.NotNullOrWhiteSpace());

        return comparer is not null
            ? filtered
            : filtered.Distinct(comparer);
    }
}
