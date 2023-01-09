namespace OneI.Collections.Generic;

using OneI.Buffers;

public interface IArraySort<T>
    where T : unmanaged
{
    void Sort(ValueBuffer<T> keys, IComparer<T>? comparer);

    int BinarySearch(T[] keys, int index, int length, T value, IComparer<T> comparer);
}
