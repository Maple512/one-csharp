namespace System.Collections.Generic;

public static class HashSetExtensions
{
    public static HashSet<T> AddAll<T>(this HashSet<T> set, IEnumerable<T> value)
    {
        foreach(var item in value)
        {
            set.Add(item);
        }

        return set;
    }
}
