namespace System.Collections.Generic;

using System.Linq;
using OneI;

/// <summary>
/// The collection extensions.
/// </summary>
#if NET7_0_OR_GREATER
[StackTraceHidden]
#endif
[DebuggerStepThrough]
internal static partial class CollectionExtensions
{
    /// <summary>
    /// Adds the if not contains.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="item">The item.</param>
    public static void AddIfNotContains<T>(this ICollection<T> source, T item)
    {
        Check.NotNull(source);

        if(!source.Contains(item))
        {
            source.Add(item);
        }
    }

    /// <summary>
    /// Adds the if not contains.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="items">The items.</param>
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

    /// <summary>
    /// Removes the all.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>A list of TS.</returns>
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
