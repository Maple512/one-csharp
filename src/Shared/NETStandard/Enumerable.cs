namespace System.Diagnostics;

internal static class Enumerable
{
    public static bool TryGetNonEnumeratedCount<TSource>(this IEnumerable<TSource> source, out int count)
    {
        if(source == null)
        {
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
        }

        if(source is ICollection<TSource> collectionoft)
        {
            count = collectionoft.Count;
            return true;
        }

        if(source is ICollection collection)
        {
            count = collection.Count;
            return true;
        }

        count = 0;

        return false;
    }
}
