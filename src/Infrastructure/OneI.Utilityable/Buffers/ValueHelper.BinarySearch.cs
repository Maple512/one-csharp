namespace OneI.Buffers;

using OneI.Buffers;

internal static partial class ValueHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int BinarySearch<T, TComparable>(ReadOnlyValueBuffer<T> span, TComparable comparable)
        where T : unmanaged
        where TComparable : IComparable<T>
    {
        if(comparable == null)
        {
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.comparable);
        }

        return BinarySearch(ref Unsafe.AsRef(span.GetReference()), span.Length, comparable);
    }

    internal static int BinarySearch<T, TComparable>(ref T spanStart, int length, TComparable comparable)
        where T : unmanaged
        where TComparable : IComparable<T>
    {
        var num = 0;
        var num2 = length - 1;
        while(num <= num2)
        {
            var num3 = (int)((uint)(num2 + num) >> 1);
            var num4 = comparable.CompareTo(Unsafe.Add(ref spanStart, num3));
            if(num4 == 0)
            {
                return num3;
            }

            if(num4 > 0)
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
}
