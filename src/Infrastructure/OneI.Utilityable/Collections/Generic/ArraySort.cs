namespace OneI.Collections.Generic;

using System.Numerics;
using OneI.Buffers;

/// <summary>
/// source: <see href="https://github.com/dotnet/runtime/blob/6009a1064ccfc2bd9aeb96c9247b60cf6352198d/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/ArraySort.cs#L15"/>
/// </summary>
/// <typeparam name="T"></typeparam>
internal sealed class ArraySort<T> : IArraySort<T>
    where T : unmanaged
{
    private static readonly IArraySort<T> s_defaultArraySortHelper = new ArraySort<T>();

    public static IArraySort<T> Default => s_defaultArraySortHelper;

    public void Sort(ValueBuffer<T> keys, IComparer<T>? comparer)
    {
        try
        {
            comparer ??= Comparer<T>.Default;

            IntrospectiveSort(keys, comparer.Compare);
        }
        catch(IndexOutOfRangeException)
        {
            ThrowHelper.ThrowArgumentException_BadComparer(comparer);
        }
        catch(Exception e)
        {
            ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_IComparerFailed, e);
        }
    }

    public int BinarySearch(T[] array, int index, int length, T value, IComparer<T> comparer)
    {
        try
        {
            comparer ??= Comparer<T>.Default;
            return InternalBinarySearch(array, index, length, value, comparer);
        }
        catch(Exception e)
        {
            ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_IComparerFailed, e);
            return 0;
        }
    }

    internal static void Sort(ValueBuffer<T> keys, Comparison<T> comparer)
    {
        try
        {
            IntrospectiveSort(keys, comparer);
        }
        catch(IndexOutOfRangeException)
        {
            ThrowHelper.ThrowArgumentException_BadComparer(comparer);
        }
        catch(Exception e)
        {
            ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_IComparerFailed, e);
        }
    }

    internal static int InternalBinarySearch(T[] array, int index, int length, T value, IComparer<T> comparer)
    {
        var num = index;
        var num2 = index + length - 1;
        while(num <= num2)
        {
            var num3 = num + (num2 - num >> 1);
            var num4 = comparer.Compare(array[num3], value);
            if(num4 == 0)
            {
                return num3;
            }

            if(num4 < 0)
            {
                num = num3 + 1;
            }
            else
            {
                num2 = num3 - 1;
            }
        }

        return ~num;
    }

    private static void SwapIfGreater(ValueBuffer<T> keys, Comparison<T> comparer, int i, int j)
    {
        if(comparer(keys[i], keys[j]) > 0)
        {
            (keys[j], keys[i]) = (keys[i], keys[j]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Swap(ValueBuffer<T> a, int i, int j)
    {
        (a[j], a[i]) = (a[i], a[j]);
    }

    internal static void IntrospectiveSort(ValueBuffer<T> keys, Comparison<T> comparer)
    {
        if(keys.Length > 1)
        {
            IntroSort(keys, 2 * (BitOperations.Log2((uint)keys.Length) + 1), comparer);
        }
    }

    private static void IntroSort(ValueBuffer<T> keys, int depthLimit, Comparison<T> comparer)
    {
        var num = keys.Length;
        while(num > 1)
        {
            if(num <= 16)
            {
                switch(num)
                {
                    case 2:
                        SwapIfGreater(keys, comparer, 0, 1);
                        break;
                    case 3:
                        SwapIfGreater(keys, comparer, 0, 1);
                        SwapIfGreater(keys, comparer, 0, 2);
                        SwapIfGreater(keys, comparer, 1, 2);
                        break;
                    default:
                        InsertionSort(keys[..num], comparer);
                        break;
                }

                break;
            }

            if(depthLimit == 0)
            {
                HeapSort(keys[..num], comparer);
                break;
            }

            depthLimit--;
            var num2 = PickPivotAndPartition(keys[..num], comparer);
            var span = keys;
            var num3 = num2 + 1;
            IntroSort(span[num3..num], depthLimit, comparer);
            num = num2;
        }
    }

    private static int PickPivotAndPartition(ValueBuffer<T> keys, Comparison<T> comparer)
    {
        var num = keys.Length - 1;
        var num2 = num >> 1;
        SwapIfGreater(keys, comparer, 0, num2);
        SwapIfGreater(keys, comparer, 0, num);
        SwapIfGreater(keys, comparer, num2, num);
        var val = keys[num2];
        Swap(keys, num2, num - 1);
        var num3 = 0;
        var num4 = num - 1;
        while(num3 < num4)
        {
            while(comparer(keys[++num3], val) < 0)
            {
            }

            while(comparer(val, keys[--num4]) < 0)
            {
            }

            if(num3 >= num4)
            {
                break;
            }

            Swap(keys, num3, num4);
        }

        if(num3 != num - 1)
        {
            Swap(keys, num3, num - 1);
        }

        return num3;
    }

    private static void HeapSort(ValueBuffer<T> keys, Comparison<T> comparer)
    {
        var length = keys.Length;
        for(var num = length >> 1; num >= 1; num--)
        {
            DownHeap(keys, num, length, comparer);
        }

        for(var num2 = length; num2 > 1; num2--)
        {
            Swap(keys, 0, num2 - 1);
            DownHeap(keys, 1, num2 - 1, comparer);
        }
    }

    private static void DownHeap(ValueBuffer<T> keys, int i, int n, Comparison<T> comparer)
    {
        var val = keys[i - 1];
        while(i <= n >> 1)
        {
            var num = 2 * i;
            if(num < n && comparer(keys[num - 1], keys[num]) < 0)
            {
                num++;
            }

            if(comparer(val, keys[num - 1]) >= 0)
            {
                break;
            }

            keys[i - 1] = keys[num - 1];
            i = num;
        }

        keys[i - 1] = val;
    }

    private static void InsertionSort(ValueBuffer<T> keys, Comparison<T> comparer)
    {
        for(var i = 0; i < keys.Length - 1; i++)
        {
            var val = keys[i + 1];
            var num = i;
            while(num >= 0 && comparer(val, keys[num]) < 0)
            {
                keys[num + 1] = keys[num];
                num--;
            }

            keys[num + 1] = val;
        }
    }
}

