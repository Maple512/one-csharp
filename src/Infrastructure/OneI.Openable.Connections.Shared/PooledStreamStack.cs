namespace OneI.Openable;

internal struct PooledStreamStack<TValue>
    where TValue : class, IPooledStream
{
    private StreamAsValueType[] _array;
    private int _size;

    public PooledStreamStack(int size)
    {
        _array = new StreamAsValueType[size];
        _size = size;
    }

    public readonly int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _size;
    }

    /// <summary>
    /// 尝试获取堆栈的顶部对象并从堆栈中移除。
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryPop([NotNullWhen(true)] out TValue? value)
    {
        var size = _size - 1;
        var array = _array;

        if((uint)size >= (uint)array.Length)
        {
            value = default;

            return false;
        }

        _size = size;
        value = array[size];
        array[size] = default;
        return true;
    }

    /// <summary>
    /// 尝试获取堆栈的顶部对象，但不会移除。
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryPeek([NotNullWhen(true)] out TValue? value)
    {
        var size = _size - 1;
        var array = _array;

        if((uint)size >= (uint)array.Length)
        {
            value = default;

            return false;
        }

        value = array[size];

        return true;
    }

    public void Push(TValue item)
    {
        int size = _size;
        var array = _array;

        if((uint)size < (uint)array.Length)
        {
            array[size] = item;
            _size = size + 1;
        }
        else
        {
            PushWithResize(item);
        }
    }

    public void RemoveExpired(long now)
    {
        var size = _size;
        var array = _array;

        var removeCount = CalculateRemoveCount(now, size, array);
        if(removeCount is 0)
        {
            return;
        }

        var newSize = size - removeCount;

        // dispose removed streams
        for(int i = 0; i < removeCount; i++)
        {
            TValue stream = array[i];

            stream.DisposeCore();
        }

        // move remaining streams
        for(int i = 0; i < newSize; i++)
        {
            array[i] = array[i + removeCount];
        }

        // clear unused array indexes
        for(int i = newSize; i < size; i++)
        {
            array[i] = default;
        }

        _size = newSize;
    }

    // 非内联，以提高其作为不常用路径的代码质量
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void PushWithResize(TValue item)
    {
        Array.Resize(ref _array, _array.Length * 2);

        _array[_size] = item;

        _size++;
    }

    private static int CalculateRemoveCount(long now, int size, StreamAsValueType[] array)
    {
        for(int i = 0; i < size; i++)
        {
            TValue stream = array[i];
            if(stream.PoolExpirationTicks >= now)
            {
                return i;
            }
        }

        return size;
    }

    //  封装一个包装器，以便绕过CLR的协变检查
    // See https://github.com/dotnet/runtime/blob/da9b16f2804e87c9c1ca9dcd9036e7b53e724f5d/src/libraries/System.IO.Pipelines/src/System/IO/Pipelines/BufferSegmentStack.cs#L68-L79
    internal readonly struct StreamAsValueType
    {
        private readonly TValue _value;

        public StreamAsValueType(TValue value)
        {
            _value = value;
        }

        public static implicit operator StreamAsValueType(TValue value) => new(value);

        public static implicit operator TValue(StreamAsValueType stream) => stream._value;
    }
}
