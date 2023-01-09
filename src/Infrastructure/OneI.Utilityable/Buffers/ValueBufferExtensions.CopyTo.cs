namespace OneI.Buffers;

public static partial class ValueBufferExtensions
{
    /// <summary>
    /// 将数组的内容复制到指定的目标容器中
    /// <para>如果源和目标重叠，则此方法的行为就像覆盖目标之前临时位置中的原始值一样</para>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo<T>(this T[]? source, ValueBuffer<T> destination)
        where T : unmanaged
    {
        new ReadOnlyValueBuffer<T>(source).CopyTo(destination);
    }

    public static unsafe void CopyTo<T>(this ReadOnlySpan<T> source, ValueBuffer<T> destination)
        where T : unmanaged
    {
        if(source.IsEmpty)
        {
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
        }

        if(source.Length > destination.Length)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException();
        }

        Buffer.MemoryCopy(
                Unsafe.AsPointer(ref MemoryMarshal.GetReference(source)),
                destination.GetPointer(),
                destination.Length * sizeof(char),
                source.Length * sizeof(char));
    }

    public static unsafe void CopyTo(this string chars, ValueBuffer<char> buffer)
    {
        if(chars is null)
        {
            ThrowHelper.ThrowArgumentNullException(ExceptionArgument.chars);
        }

        if(chars.Length > buffer.Length)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException();
        }

        const int size = sizeof(char);

        Buffer.MemoryCopy(
            Unsafe.AsPointer(ref Unsafe.AsRef(chars.GetPinnableReference())),
            buffer.GetPointer(),
            buffer.Length * size,
            (uint)chars.Length * size);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool TryCopyTo(this ReadOnlySpan<char> value, ValueBuffer<char> destination)
    {
        var result = false;
        if(value.IsEmpty == false && (uint)value.Length <= (uint)destination.Length)
        {
            Buffer.MemoryCopy(
                Unsafe.AsPointer(ref MemoryMarshal.GetReference(value)),
                destination.GetPointer(),
                destination.Length * sizeof(char),
                value.Length * sizeof(char));

            result = true;
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool TryCopyTo(this Span<char> value, ValueBuffer<char> destination)
    {
        var result = false;
        if(value.IsEmpty == false && (uint)value.Length <= (uint)destination.Length)
        {
            Buffer.MemoryCopy(
                Unsafe.AsPointer(ref MemoryMarshal.GetReference(value)),
                destination.GetPointer(),
                destination.Length * sizeof(char),
                value.Length * sizeof(char));

            result = true;
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe bool TryCopyTo(this string value, ValueBuffer<char> destination)
    {
        var result = false;
        if(value.IsNullOrEmpty() == false && (uint)value.Length <= (uint)destination.Length)
        {
            Buffer.MemoryCopy(
                Unsafe.AsPointer(ref Unsafe.AsRef(value.GetPinnableReference())),
                destination.GetPointer(),
                destination.Length * sizeof(char),
                value.Length * sizeof(char));

            result = true;
        }

        return result;
    }

    /// <summary>
    /// Creates a new span over the portion of the target array.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueBuffer<T> AsValueBuffer<T>(this T[]? array, int start)
        where T : unmanaged
    {
        if(array == null)
        {
            if(start != 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException();
            }

            return default;
        }

        if(!typeof(T).IsValueType && array.GetType() != typeof(T[]))
        {
            ThrowHelper.ThrowArrayTypeMismatchException();
        }

        if((uint)start > (uint)array.Length)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException();
        }

        return new ValueBuffer<T>(ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), (nint)(uint)start /* force zero-extension */), array.Length - start);
    }

    /// <summary>
    /// Creates a new span over the portion of the target array.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueBuffer<T> AsValueBuffer<T>(this T[]? array, Index startIndex)
        where T : unmanaged
    {
        if(array == null)
        {
            if(!startIndex.Equals(Index.Start))
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            }

            return default;
        }

        if(!typeof(T).IsValueType && array.GetType() != typeof(T[]))
        {
            ThrowHelper.ThrowArrayTypeMismatchException();
        }

        var actualIndex = startIndex.GetOffset(array.Length);
        if((uint)actualIndex > (uint)array.Length)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException();
        }

        return new ValueBuffer<T>(ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array),
            (nint)(uint)actualIndex /* force zero-extension */), array.Length - actualIndex);
    }

    /// <summary>
    /// Creates a new span over the portion of the target array.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueBuffer<T> AsValueBuffer<T>(this T[]? array, Range range)
        where T : unmanaged
    {
        if(array == null)
        {
            var startIndex = range.Start;
            var endIndex = range.End;

            if(!startIndex.Equals(Index.Start) || !endIndex.Equals(Index.Start))
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            }

            return default;
        }

        if(!typeof(T).IsValueType && array.GetType() != typeof(T[]))
        {
            ThrowHelper.ThrowArrayTypeMismatchException();
        }

        (var start, var length) = range.GetOffsetAndLength(array.Length);
        return new ValueBuffer<T>(ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), (nint)(uint)start /* force zero-extension */), length);
    }

    /// <summary>
    /// Creates a new readonly span over the portion of the target string.
    /// </summary>
    /// <param name="text">The target string.</param>
    /// <remarks>Returns default when <paramref name="text"/> is null.</remarks>
    [Intrinsic]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyValueBuffer<char> AsValueBuffer(this string? text)
    {
        if(text == null)
        {
            return default;
        }

        return new ReadOnlyValueBuffer<char>(ref Unsafe.AsRef(text.GetPinnableReference()), text.Length);
    }

    /// <summary>
    /// Creates a new readonly span over the portion of the target string.
    /// </summary>
    /// <param name="text">The target string.</param>
    /// <param name="start">The index at which to begin this slice.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Thrown when the specified <paramref name="start"/> index is not in range (&lt;0 or &gt;text.Length).
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyValueBuffer<char> AsValueBuffer(this string? text, int start)
    {
        if(text == null)
        {
            if(start != 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
            }

            return default;
        }

        if((uint)start > (uint)text.Length)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
        }

        return new ReadOnlyValueBuffer<char>(ref Unsafe.Add(ref Unsafe.AsRef(text.GetPinnableReference()), (nint)(uint)start /* force zero-extension */), text.Length - start);
    }

    /// <summary>
    /// Creates a new readonly span over the portion of the target string.
    /// </summary>
    /// <param name="text">The target string.</param>
    /// <param name="start">The index at which to begin this slice.</param>
    /// <param name="length">The desired length for the slice (exclusive).</param>
    /// <remarks>Returns default when <paramref name="text"/> is null.</remarks>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Thrown when the specified <paramref name="start"/> index or <paramref name="length"/> is not in range.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyValueBuffer<char> AsValueBuffer(this string? text, int start, int length)
    {
        if(text == null)
        {
            if(start != 0 || length != 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
            }

            return default;
        }

#if TARGET_64BIT
            // See comment in ValueBuffer<T>.Slice for how this works.
            if ((ulong)(uint)start + (ulong)(uint)length > (ulong)(uint)text.Length)
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
#else
        if((uint)start > (uint)text.Length || (uint)length > (uint)(text.Length - start))
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
        }
#endif

        return new ReadOnlyValueBuffer<char>(ref Unsafe.Add(ref Unsafe.AsRef(text.GetPinnableReference()), (nint)(uint)start /* force zero-extension */), length);
    }

    /// <summary>
    /// Creates a new span over the target array.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueBuffer<T> AsValueBuffer<T>(this T[]? array)
        where T : unmanaged
    {
        return new ValueBuffer<T>(array);
    }

    /// <summary>
    /// Creates a new Span over the portion of the target array beginning
    /// at 'start' index and ending at 'end' index (exclusive).
    /// </summary>
    /// <param name="array">The target array.</param>
    /// <param name="start">The index at which to begin the Span.</param>
    /// <param name="length">The number of items in the Span.</param>
    /// <remarks>Returns default when <paramref name="array"/> is null.</remarks>
    /// <exception cref="System.ArrayTypeMismatchException">Thrown when <paramref name="array"/> is covariant and array's type is not exactly T[].</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Thrown when the specified <paramref name="start"/> or end index is not in the range (&lt;0 or &gt;Length).
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueBuffer<T> AsValueBuffer<T>(this T[]? array, int start, int length)
        where T : unmanaged
    {
        return new ValueBuffer<T>(array, start, length);
    }

    /// <summary>
    /// Creates a new span over the portion of the target array segment.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueBuffer<T> AsValueBuffer<T>(this ArraySegment<T> segment)
        where T : unmanaged
    {
        return new ValueBuffer<T>(segment.Array, segment.Offset, segment.Count);
    }

    /// <summary>
    /// Creates a new Span over the portion of the target array beginning
    /// at 'start' index and ending at 'end' index (exclusive).
    /// </summary>
    /// <param name="segment">The target array.</param>
    /// <param name="start">The index at which to begin the Span.</param>
    /// <remarks>Returns default when <paramref name="segment"/> is null.</remarks>
    /// <exception cref="System.ArrayTypeMismatchException">Thrown when <paramref name="segment"/> is covariant and array's type is not exactly T[].</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Thrown when the specified <paramref name="start"/> or end index is not in the range (&lt;0 or &gt;segment.Count).
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueBuffer<T> AsValueBuffer<T>(this ArraySegment<T> segment, int start)
        where T : unmanaged
    {
        if((uint)start > (uint)segment.Count)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
        }

        return new ValueBuffer<T>(segment.Array, segment.Offset + start, segment.Count - start);
    }

    /// <summary>
    /// Creates a new Span over the portion of the target array beginning
    /// at 'startIndex' and ending at the end of the segment.
    /// </summary>
    /// <param name="segment">The target array.</param>
    /// <param name="startIndex">The index at which to begin the Span.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueBuffer<T> AsValueBuffer<T>(this ArraySegment<T> segment, Index startIndex)
        where T : unmanaged
    {
        var actualIndex = startIndex.GetOffset(segment.Count);
        return AsValueBuffer(segment, actualIndex);
    }

    /// <summary>
    /// Creates a new Span over the portion of the target array beginning
    /// at 'start' index and ending at 'end' index (exclusive).
    /// </summary>
    /// <param name="segment">The target array.</param>
    /// <param name="start">The index at which to begin the Span.</param>
    /// <param name="length">The number of items in the Span.</param>
    /// <remarks>Returns default when <paramref name="segment"/> is null.</remarks>
    /// <exception cref="System.ArrayTypeMismatchException">Thrown when <paramref name="segment"/> is covariant and array's type is not exactly T[].</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Thrown when the specified <paramref name="start"/> or end index is not in the range (&lt;0 or &gt;segment.Count).
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueBuffer<T> AsValueBuffer<T>(this ArraySegment<T> segment, int start, int length)
        where T : unmanaged
    {
        if((uint)start > (uint)segment.Count)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
        }

        if((uint)length > (uint)(segment.Count - start))
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.length);
        }

        return new ValueBuffer<T>(segment.Array, segment.Offset + start, length);
    }

    /// <summary>
    /// Creates a new Span over the portion of the target array using the range start and end indexes
    /// </summary>
    /// <param name="segment">The target array.</param>
    /// <param name="range">The range which has start and end indexes to use for slicing the array.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueBuffer<T> AsValueBuffer<T>(this ArraySegment<T> segment, Range range)
        where T : unmanaged
    {
        (var start, var length) = range.GetOffsetAndLength(segment.Count);

        return new ValueBuffer<T>(segment.Array, segment.Offset + start, length);
    }
}
