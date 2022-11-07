namespace System.Collections.Generic;

using System;
using System.Diagnostics;
using System.Linq;
using OneI;

[StackTraceHidden]
[DebuggerStepThrough]
public static class CollectionExtensions
{
    public static void AddIfNotContains<T>(this ICollection<T> source, T item)
    {
        CheckTools.NotNull(source);

        if(!source.Contains(item))
        {
            source.Add(item);
        }
    }

    public static void AddIfNotContains<T>(this ICollection<T> source, IEnumerable<T> items)
    {
        CheckTools.NotNull(source);

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
