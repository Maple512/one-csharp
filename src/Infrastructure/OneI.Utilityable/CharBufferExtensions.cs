namespace OneI;

using DotNext.Runtime;

public static class CharBufferExtensions
{
    /// <summary>
    /// 搜索指定的值，如果找到则返回true
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool Contains<T>(this ValueBuffer<T> buffer, char value)
        where T : unmanaged
    {
        foreach(var item in buffer)
        {
            if(item.Equals(value))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 搜索指定值并返回其第一次出现的索引
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int IndexOf<T>(this ValueBuffer<T> buffer, char value)
        where T : unmanaged
    {
        var index = 0;

        foreach(var item in buffer)
        {
            if(item.Equals(value))
            {
                return index;
            }

            index++;
        }

        return -1;
    }

    /// <summary>
    /// 搜索指定值并返回其最后一次出现的索引
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int LastIndexOf<T>(this ValueBuffer<T> buffer, char value)
        where T : unmanaged
    {
        var index = buffer.Length;

        while(index > 0)
        {
            if(buffer[index - 1].Equals(value))
            {
                return index;
            }

            index--;
        }

        return -1;
    }

    /// <summary>
    /// 确定两个序列是否相等
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool SequenceEquals<T>(this ValueBuffer<T> buffer, ValueBuffer<T> other)
        where T : unmanaged
    {
        if(buffer.Length != other.Length)
        {
            return false;
        }

        return SequenceEquals(buffer, other, other.Length);
    }

    /// <summary>
    /// 确定两个序列在指定的长度内是否相等
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="other"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static bool SequenceEquals<T>(this ValueBuffer<T> buffer, ValueBuffer<T> other, int length)
        where T : unmanaged
    {
        if(buffer.Length < length
            || other.Length < length)
        {
            return false;
        }

        var index = 0;
        while(index < other.Length)
        {
            if(other[index].Equals(buffer[index++]) == false)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 确定指定的缓冲区是否显示在<see cref="ValueBuffer{T}"/>的开头
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool StartsWith<T>(this ValueBuffer<T> buffer, ValueBuffer<T> other)
        where T : unmanaged
    {
        new char[1].AsSpan();
        return buffer.Length >= other.Length
            && SequenceEquals(buffer, other, other.Length);
    }

    /// <summary>
    /// 确定指定的缓冲区是否显示在<see cref="ValueBuffer{T}"/>的末尾
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool EndsWith<T>(this ValueBuffer<T> buffer, ValueBuffer<T> other)
        where T : unmanaged
    {
        return buffer.Length >= other.Length
            && SequenceEquals(buffer[(other.Length - 1)..], other, other.Length);
    }

    /// <summary>
    /// 反转给定<see cref="ValueBuffer{T}"/>的所有元素
    /// </summary>
    /// <param name="buffer"></param>
    public static unsafe void Reverse<T>(this ValueBuffer<T> buffer)
        where T : unmanaged
    {
        if(buffer.IsEmpty is false)
        {
            var first = buffer.GetPointer(0);
            var last = buffer.GetPointer(buffer.Length - 1);

            do
            {
                (*last, *first) = (*first, *last);

                first++;
                last--;
            } while(first < last);
        }
    }

    /// <summary>
    /// 将数组的内容复制到指定的<paramref name="buffer"/>
    /// <para>如果源和目标重叠，则此方法的行为就像覆盖目标之前临时位置中的原始值一样</para>
    /// </summary>
    /// <param name="array"></param>
    /// <param name="buffer"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static unsafe void CopyTo<T>(this T[] array, ValueBuffer<T> buffer)
        where T : unmanaged
    {
        if(array is null)
        {
            ThrowArgumentNullException();
        }

        if(array.Length > buffer.Length)
        {
            ThrowArgumentOutOfRangeException();
        }

        if(array.Length <= buffer.Length)
        {
            var size = Unsafe.SizeOf<T>();

            Buffer.MemoryCopy(
                Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(array)),
                buffer.GetPointer(0),
                buffer.Length * size,
                array.Length * size);
        }
    }

    public static unsafe void CopyTo(this string chars, ValueBuffer<char> buffer)
    {
        if(chars is null)
        {
            ThrowArgumentNullException();
        }

        if(chars.Length > buffer.Length)
        {
            ThrowArgumentOutOfRangeException();
        }

        fixed(char* ptr = &chars.GetPinnableReference())
        {
            const int size = sizeof(char);

            Buffer.MemoryCopy(
                ptr,
                buffer.GetPointer(0),
                buffer.Length * size,
                (uint)chars.Length * size);
        }
    }

    public static unsafe void CopyTo<T>(this ReadOnlySpan<T> array, ValueBuffer<T> buffer)
        where T : unmanaged
    {
        if(array.IsEmpty)
        {
            ThrowArgumentNullException();
        }

        if(array.Length > buffer.Length)
        {
            ThrowArgumentOutOfRangeException();
        }

        var reference = array.GetPinnableReference();

        int size = Unsafe.SizeOf<T>();

        Buffer.MemoryCopy(
                Unsafe.AsPointer(ref reference),
                buffer.GetPointer(0),
                buffer.Length * size,
                array.Length * size);
    }

    [DoesNotReturn]
    static void ThrowArgumentNullException() => throw new ArgumentNullException();

    [DoesNotReturn]
    static void ThrowArgumentOutOfRangeException() => throw new ArgumentOutOfRangeException();
}
