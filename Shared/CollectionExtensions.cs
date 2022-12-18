namespace System.Collections.Generic;

using System.Linq;
using OneI;

#if NET7_0_OR_GREATER
[StackTraceHidden]
#endif

[DebuggerStepThrough]
internal static partial class CollectionExtensions
{
    public static void AddIfNotContains<T>(this ICollection<T> source, T item)
    {
        Check.NotNull(source);

        if(!source.Contains(item))
        {
            source.Add(item);
        }
    }

    public static void AddIfNotContains<T>(this ICollection<T> source, IEnumerable<T> items)
    {
        Check.NotNull(source);

        foreach(var item in items)
        {
            if(!source.Contains(item))
            {
                source.Add(item);
            }
        }
    }

    public static IEnumerable<T> RemoveAll<T>(this ICollection<T> source, Func<T, bool> predicate)
    {
        var items = source.Where(predicate).ToArray();

        foreach(var item in items)
        {
            source.Remove(item);
        }

        return items;
    }
}
