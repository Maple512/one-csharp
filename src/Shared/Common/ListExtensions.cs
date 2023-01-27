namespace System.Collections.Generic;

using OneI;

/// <summary>
/// The list extensions.
/// </summary>
internal static class ListExtensions
{
    /// <summary>
    /// Tries the find first index.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="selector">The selector.</param>
    /// <param name="index">The index.</param>
    /// <returns>A bool.</returns>
    public static bool TryFindFirstIndex<T>(this IList<T> source, Predicate<T> selector, out int index)
    {
        for(var i = 0; i < source.Count; ++i)
        {
            if(selector(source[i]))
            {
                index = i;

                return true;
            }
        }

        index = -1;

        return false;
    }

    /// <summary>
    /// Replaces the all.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="selector">The selector.</param>
    /// <param name="item">The item.</param>
    public static void ReplaceAll<T>(this IList<T> source, Predicate<T> selector, T item)
    {
        for(var i = 0; i < source.Count; i++)
        {
            if(selector(source[i]))
            {
                source[i] = item;
            }
        }
    }

    /// <summary>
    /// Replaces the all.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="selector">The selector.</param>
    /// <param name="itemFactory">The item factory.</param>
    public static void ReplaceAll<T>(this IList<T> source, Predicate<T> selector, Func<T, T> itemFactory)
    {
        for(var i = 0; i < source.Count; i++)
        {
            var item = source[i];
            if(selector(item))
            {
                source[i] = itemFactory(item);
            }
        }
    }

    /// <summary>
    /// Replaces the item.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="selector">The selector.</param>
    /// <param name="newItem">The new item.</param>
    public static void ReplaceItem<T>(this IList<T> source, Predicate<T> selector, T newItem)
    {
        for(var i = 0; i < source.Count; i++)
        {
            if(selector(source[i]))
            {
                source[i] = newItem;
                return;
            }
        }
    }

    /// <summary>
    /// Replaces the item.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="selector">The selector.</param>
    /// <param name="newItemFactory">The new item factory.</param>
    public static void ReplaceItem<T>(this IList<T> source, Predicate<T> selector, Func<T, T> newItemFactory)
    {
        for(var i = 0; i < source.Count; i++)
        {
            var item = source[i];
            if(selector(item))
            {
                source[i] = newItemFactory(item);
                return;
            }
        }
    }

    /// <summary>
    /// Replaces the item.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="item">The item.</param>
    /// <param name="newItem">The new item.</param>
    public static void ReplaceItem<T>(this IList<T> source, T item, T newItem)
    {
        for(var i = 0; i < source.Count; i++)
        {
            if(Comparer<T>.Default.Compare(source[i], item) == 0)
            {
                source[i] = newItem;
                return;
            }
        }
    }

    /// <summary>
    /// Moves the first item to.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="selector">The selector.</param>
    /// <param name="targetIndex">The target index.</param>
    public static void MoveFirstItemTo<T>(this List<T> source, Predicate<T> selector, int targetIndex)
    {
        if(targetIndex < 0
            || targetIndex > source.Count - 1)
        {
            throw new IndexOutOfRangeException($"The {nameof(targetIndex)}({targetIndex}) should be between 0 and {source.Count - 1}");
        }

        var firstItemIndex = source.FindIndex(0, selector);
        if(firstItemIndex == targetIndex)
        {
            return;
        }

        var item = source[firstItemIndex];

        source.RemoveAt(firstItemIndex);

        source.Insert(targetIndex, item);
    }

    /// <summary>
    /// Gets the or add.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="selector">The selector.</param>
    /// <param name="factory">The factory.</param>
    /// <returns>A T.</returns>
    public static T GetOrAdd<T>(this IList<T> source, Func<T, bool> selector, Func<T> factory)
    {
        Check.NotNull(source);

        var item = source.FirstOrDefault(selector);

        if(item == null)
        {
            item = factory();

            source.Add(item);
        }

        return item;
    }
}
