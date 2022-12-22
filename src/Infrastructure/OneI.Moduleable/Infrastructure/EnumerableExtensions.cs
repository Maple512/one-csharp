namespace OneI.Moduleable.Infrastructure;

using System;
using System.Collections.Generic;
/// <summary>
/// The enumerable extensions.
/// </summary>

internal static class EnumerableExtensions
{
    /// <summary>
    /// Sorts the by dependencies.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="getDependencies">The get dependencies.</param>
    /// <param name="comparer">The comparer.</param>
    /// <returns>A list of TS.</returns>
    public static List<T> SortByDependencies<T>(
        this IEnumerable<T> source,
        Func<T, IEnumerable<T>> getDependencies,
        IEqualityComparer<T>? comparer = null)
        where T : notnull
    {
        /* See: http://www.codeproject.com/Articles/869059/Topological-sorting-in-Csharp
         *      http://en.wikipedia.org/wiki/Topological_sorting
         */

        var sorted = new List<T>();
        var visited = new Dictionary<T, bool>(comparer);

        foreach(var item in source)
        {
            SortByDependenciesVisit(item, getDependencies, sorted, visited);
        }

        return sorted;
    }

    /// <summary>
    /// Sorts the by dependencies visit.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="getDependencies">The get dependencies.</param>
    /// <param name="sorted">The sorted.</param>
    /// <param name="visited">The visited.</param>
    private static void SortByDependenciesVisit<T>(
        T item,
        Func<T, IEnumerable<T>> getDependencies,
        List<T> sorted,
        Dictionary<T, bool> visited)
        where T : notnull
    {
        var alreadyVisited = visited.TryGetValue(item, out var inProcess);

        if(alreadyVisited)
        {
            if(inProcess)
            {
                throw new ArgumentException("Cyclic dependency found! Item: " + item);
            }
        }
        else
        {
            visited[item] = true;

            var dependencies = getDependencies(item);
            if(dependencies != null)
            {
                foreach(var dependency in dependencies)
                {
                    SortByDependenciesVisit(dependency, getDependencies, sorted, visited);
                }
            }

            visited[item] = false;

            sorted.Add(item);
        }
    }
}
